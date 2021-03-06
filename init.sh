#!/usr/bin/env bash

# ToDo ask all user informations in the beginning

#git clone https://github.com/frohlichcortezh/bash-scripts.git $HOME/dev/shell-scripts/
git clone https://github.com/frohlichcortezh/fish-functions.git $HOME/dev/shell-scripts/

source ../bash-scripts/functions.sh

# Add needed repositories

source setup-repositories.sh

# Install apps

f_pkg_manager_update

# ToDo break it into different categories and according to type of installation
f_app_install refind keepassxc snapd tightvncserver net-tools xrdp ssh gdebi gufw gnome-tweaks guake gnome-shell-extensions firefox python3-nautilus python3-pip apt-transport-https software-properties-common vino
f_app_install_from_snap code --classic
f_app_install_from_snap spotify
# https://github.com/flozz/nautilus-terminal
f_app_install_from_pip nautilus_terminal

#ToDo find url according to distro and version
wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O $HOME/dev/packages/packages-microsoft-prod.deb
sudo dpkg -i $HOME/dev/packages/packages-microsoft-prod.deb

f_pkg_manager_update

f_app_install dotnet-sdk-3.1

dotnet tool install -g dotnet-script

dotnet script main.csx

# setup
# inform user of actions to do 

#configure ssh

# enable services
sudo systemctl enable xrdp


# manual actions

firefox https://extensions.gnome.org/extension/906/sound-output-device-chooser/

# gnome alt+tab switch windows not group of windows
# https://techwiser.com/ubuntu-alt-tab-ungroup/
gsettings set org.gnome.desktop.wm.keybindings switch-windows "['<Alt>Tab']"