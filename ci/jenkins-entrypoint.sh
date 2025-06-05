#!/bin/bash

# Run the Git initialization script
echo "Running Git initialization script..."
/usr/local/bin/init-git.sh

# Start Jenkins with the original entrypoint
echo "Starting Jenkins..."
exec /usr/local/bin/jenkins.sh "$@"
