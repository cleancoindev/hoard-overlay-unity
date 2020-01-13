# Intro #

Hi and hello to Hoard Overlay Unity package homepage.

## [API](https://hoard.exchange/hoard-unity-overlay)
If you need, all API reference is available under this link. 

## What is Hoard Overlay ##

Hoard overlay is a Unity based project that will help you with avoid boiler-plating Hoard Profiles management code.
This project is a base for your own solution and aims to be hackable and extendable for your needs. 

## What's in the package ##

Out of the box you get a working scene with overlay that can be used in your project. It has following functionality:

- Create Hoard Profiles
- Change Hoard Profiles 
- Store Profiles locally on the user storage
- Transfer accounts between user devices using Whisper
- Start Hoard Service and display user HRD token amount

### Supported Input ###
The example overlay can be used out of the box with
1. Standard mouse and keyboard combo
2. Touch devices
3. GamePad - The text input on the desktop platforms have to be still provided by keyboard

### Platforms ###

This package is intended for Unity.2018+ with .Net4.x enabled. Code will work on following platforms:
1. Windows 
2. Linux
3. Android 7+ 

We have plans for for iOS and osX support but right now we cant guarantee compatibility.

# Project Structure #

Project on GitHub on directories and namespace level splits into two modules:

## MVC // namespace [Hoard.MVC] ##

This part contains all the logic of the system along with the view controllers used for UI. It's located in Assets/HoardOverlay/MVC directory.
It does not contain any graphical representation and is completely uncoupled from it. You can use this part of the code if you want to create completely new UI Views for your user that follow the logic of your in game GUI System.

You can also use this code to create the scaffolding for the project that is not using Unity. 

### Dependencies ###

This namespace and code does not rely on any Unity or Hoard.MVC.Unity code and can be imported/copied even to other Mono Project. Feel free to experiment :) 
All the dlls referenced by this namespace can be found in _/HoardOverlay/MVC/Plugins_ directory.

## Hoard Unity MVC // namespace [Hoard.MVC.Unity] ##

Contains Views and Unity specific code. It contains all the code that allow display of the data and receiving the input from the player and pass it down. 

### Dependencies ###

The GUI of the views are depended on the *Text_Mesh_Pro* unity package.
This code depends on [Hoard.MVC](#Hoard.MVC) namespace.

# Hoard Profiles #

Provided tools allow user to create and manage Hoard Profiles. Hoard profiles are way of storing password encrypted 'Ethereum' wallet.

## Profiles storage ##

We stand on position that blockchain technology should be decentralized therefor Hoard is NOT storing user wallets and encryptions keys in any way. All the keys are stored by the user on the user's machines. It's important from the UX perspective to remind user of this fact. We strongly descourage storing user passwords or encryption keys.

## Profiles Transfer ##

To assure user's privacy with provide you with whisper transfer of encoded profiles. While the profiles are being transfered the passwords should NEVER! be transfered along with them to avoid P2P attack. Procedure is based on the assumption that user has controled physical access to both devices while profile is being transfered.

# More information on HoardSDK #

## Hoard SDK GitHub ##

You can extend blockchain integration of the game using included HoardSDK.
You can find more information on the project [Github](https://github.com/hoardexchange/HoardSDK)

## Email us ##

Contact as via [email](mailto:hello@hoard.exchange)

## Check our [website](https://www.hoard.exchange/) ##
