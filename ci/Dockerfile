FROM jenkins/jenkins:lts

USER root

# Install dependencies
RUN apt-get update && apt-get install -y \
    apt-transport-https \
    ca-certificates \
    curl \
    gnupg \
    lsb-release \
    git \
    unzip \
    wget \
    default-jre

# Install .NET SDK 8.0
RUN wget https://packages.microsoft.com/config/debian/11/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y dotnet-sdk-8.0

# Install Docker CLI
RUN curl -fsSL https://download.docker.com/linux/debian/gpg | gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg && \
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/debian \
    $(lsb_release -cs) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null && \
    apt-get update && apt-get install -y docker-ce-cli

# Configure Git for Jenkins SCM operations
RUN git config --system --add safe.directory "*" && \
    mkdir -p /var/jenkins_home/workspace && \
    chown -R jenkins:jenkins /var/jenkins_home/workspace

# Install Allure commandline tool
ENV ALLURE_HOME=/opt/allure
RUN curl -o allure-2.24.1.tgz -Ls https://github.com/allure-framework/allure2/releases/download/2.24.1/allure-2.24.1.tgz && \
    mkdir -p ${ALLURE_HOME} && \
    tar -zxvf allure-2.24.1.tgz -C /tmp && \
    cp -R /tmp/allure-2.24.1/* ${ALLURE_HOME}/ && \
    rm allure-2.24.1.tgz && \
    rm -rf /tmp/allure-2.24.1 && \
    ln -s ${ALLURE_HOME}/bin/allure /usr/bin/allure && \
    chmod -R 755 ${ALLURE_HOME} && \
    allure --version

# Add Git initialization script
COPY init-git.sh /usr/local/bin/init-git.sh
RUN chmod +x /usr/local/bin/init-git.sh

# Switch back to jenkins user
USER jenkins

# Copy plugins list and install plugins
COPY plugins.txt /usr/share/jenkins/ref/plugins.txt
RUN jenkins-plugin-cli --plugin-file /usr/share/jenkins/ref/plugins.txt

# We'll configure Allure manually instead of using Configuration as Code

# Set up environment variables
ENV JAVA_OPTS="-Djenkins.install.runSetupWizard=false"

# Create entrypoint wrapper script
USER root
COPY jenkins-entrypoint.sh /usr/local/bin/jenkins-entrypoint.sh
RUN chmod +x /usr/local/bin/jenkins-entrypoint.sh
USER jenkins

# Set the entrypoint
ENTRYPOINT ["/usr/local/bin/jenkins-entrypoint.sh"]
