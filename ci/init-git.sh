#!/bin/bash

# This script initializes Git repositories for Jenkins jobs
# to fix the "not in a git directory" error when using SCM checkout

# Create workspace directory if it doesn't exist
mkdir -p /var/jenkins_home/workspace

# Function to initialize Git in a directory
init_git_in_dir() {
  local dir=$1
  echo "Checking directory: $dir"
  
  # Only initialize if not already a Git repository
  if [ -d "$dir" ] && [ ! -d "$dir/.git" ]; then
    echo "Initializing Git repository in $dir"
    cd "$dir"
    git init
    git config --local --add safe.directory "$dir"
  fi
}

# Initialize Git in the current workspace if specified
if [ -n "$WORKSPACE" ]; then
  init_git_in_dir "$WORKSPACE"
fi

# Also check common workspace locations
for workspace_dir in /var/jenkins_home/workspace/*; do
  if [ -d "$workspace_dir" ]; then
    init_git_in_dir "$workspace_dir"
  fi
done

echo "Git initialization completed"
