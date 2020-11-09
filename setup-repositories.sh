#!/usr/bin/env bash

#-----------------------------------------------------------------------------------
# Add needed repositories for distro
#-----------------------------------------------------------------------------------
source ../bash-scripts/app-management.sh

    repo_added=0

    if ! f_repository_is_installed "universe"; then
        f_repository_add universe
    fi

    if ! f_repository_is_installed "refind"; then
        f_repository_add ppa:rodsmith/refind
    fi

    if ! f_repository_is_installed "cli.github"; then
        sudo apt-key adv --keyserver keyserver.ubuntu.com --recv-key C99B11DEB97541F0
        f_repository_add https://cli.github.com/packages
    fi
    
    if [ repo_added=1 ]; then
        f_pkg_manager_update
    fi