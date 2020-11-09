#!/usr/bin/env bash

mkdir $HOME/dev
mkdir $HOME/dev/shell-scripts
mkdir $HOME/packages
mkdir $HOME/applications

# ToDo break it into different files

# ToDo ask all user informations in the beginning

#git clone https://github.com/frohlichcortezh/bash-scripts.git $HOME/dev/shell-scripts/
git clone https://github.com/frohlichcortezh/fish-functions.git $HOME/dev/shell-scripts/

source ../bash-scripts/functions.sh

# Add needed repositories

source setup-repositories.sh

# Install apps

f_pkg_manager_update

# ToDo break it into different categories and according to type of installation
f_app_install refind keepassxc snapd

f_app_install_from_snap code
