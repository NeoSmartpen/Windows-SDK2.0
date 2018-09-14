# C# Software Development Kit for Neo smartpen

## Windows SDK 2.0 

Windows SDK for Windows. This open-source library allows you to integrate the Neo smartpen - Neo smartpen N2 and M1 - into your Windows program.  

## About Neo smartpen 

The Neo smartpen is designed to seamlessly integrate the real and digital worlds by transforming what you write on paper - everything from sketches and designs to business meeting notes - to your iOS, Android and Windows devices. It works in tandem with N notebooks, powered by NeoLAB Convergence’s patented Ncode™ technology and the accompanying application, Neo Notes. Find out more at www.neosmartpen.com
- Tutorial video - https://goo.gl/MQaVwY

## About Ncode™: service development guide 

‘Natural Handwriting’ technology based on Ncode™(Microscopic data patterns containing various types of data) is a handwriting stroke recovery technology that digitizes paper coordinates obtained by optical pen devices such as Neo smartpen. The coordinates then can be used to store handwriting stroke information, analyzed to extract meaning based on user preferences and serve as the basis for many other types of services. 

Click the link below to view a beginners guide to Ncode technology. 
[https://github.com/NeoSmartpen/Documentations/blob/master/Ncode™ Service Development Getting Started Guide v1.01.pdf](https://github.com/NeoSmartpen/Documentations/blob/master/Ncode%E2%84%A2%20Service%20Development%20Getting%20Started%20Guide%20v1.01.pdf)

## Requirements

-  Windows 7 or later ( Not Windows Store Apps )
-  Microsoft Visual Studio 2013 or later
-  Microsoft .NET Framework 4 or later
-  Standard Bluetooth Dongles ( Bluetooth Specification Version 2.1 + EDR or later with Microsoft Bluetooth stack )
-  SDK packages for Neo smartpen

## Dependencies

- InTheHand.Net.Personal.dll ( v3.5 )
- Ionic.Zip.Reduced.dll ( v1.9 )

## Supported models

- Neo smartpen N2(F110, F120)
- Neo smartpen M1(F50)

## SDK Structure

| Namespace | Description |
|:---|:--------|
| Neosmartpen.Net | Framework providing interface for communication |
| Neosmartpen.Net.Bluetooth | Provide features for controlling Bluetooth communication |
| Neosmartpen.Net.Support | Provide features except communication ( such as graphics ) |
| Neosmartpen.Net.Protocol.v1 | Handling data and communication with peer device ( protocol version is 1.xx )   |
| Neosmartpen.Net.Protocol.v2 | Handling data and communication with peer device ( protocol version is 2.xx ) |

## Getting started (Basic) 

#### Create a BluetoothAdapter instance

```cs
BluetoothAdapter mBtAdt = new BluetoothAdapter();
```

#### How to discover peer device

You can discover near device in a state of discorable

```cs
PenDevice[] devices = mBtAdt.FindAllDevices();
```

#### Connecting to peer device

```cs
// Device's identifier
string mac = "9C7BD2FFF014";

// You can obtain deviceClass value by delegate callback when your bluetooth connection is established.
// BluetoothAdapter create a socket. you should implements code to bind the socket with proper PenComm.
bool result = mBtAdt.Connect( mac, delegate( uint deviceClass )
{
...
} );
```

#### Create a PenComm instance

Create a PenComm instance Handling data and communication with peer device.
You should create suitable PenComm for device's protocol version.

```cs
// PenComm for Protocol Version 1.xx ( You should implements PenCommV1Callbacks interface to your parent class )
PenCommV1 mPenCommV1 = new PenCommV1( PenCommV1Callbacks );

// PenComm for Protocol Version 2.xx ( You should implements PenCommV2Callbacks interface to your parent class )
PenCommV1 mPenCommV2 = new PenCommV2( PenCommV2Callbacks );
```

#### Bluetooth Socket bind to specific PenComm class

```cs
// Device's identifier
string mac = "9C7BD2FFF014";

// You can obtain deviceClass value by delegate callback when your bluetooth connection is established.
// BluetoothAdapter create a socket. you should implements code to bind the socket with proper PenComm.
bool result = mBtAdt.Connect( mac, delegate( uint deviceClass )
{
    // if protocol version is 1.xx
    if ( deviceClass == mPenCommV1.DeviceClass )
    {
        mBtAdt.Bind( mPenCommV1 );

        // You can set the name of PenComm object in the following ways
        // If you don't set the name of the PenComm, it is automatically set to the address of a connected pen.
        mBtAdt.Bind( mPenCommV1, "name of PenComm" );

        // You can get or set a name of PenComm
        mBtAdt.Name = "name of PenComm";
    }
    // if protocol version is 2.xx
    else if ( deviceClass == mPenCommV2.DeviceClass )
    {
        mBtAdt.Bind( mPenCommV2 );
    }
} );
```

#### After Connection is established.
**PenCommV1**

```cs
// It is occured when connection is established ( You cannot use function on your device without authentication )
// sender : sender refers to the object that invoked the callback method
// maxForce : maximum level of force sensor
// swVersion : version of firmware
void PenCommV1Callbacks.onConnected( IPenComm sender, int maxForce, string swVersion )
{
	// You can obtain the PenComm object that invoked the callback method in the below ways
    // If your application need to be allow multiple connection, sender parameter will be helpful
    PenCommV1 pencomm = sender as PenCommV1;
}

// If your device is locked, it is called to input password.
// retryCount : Number of password verification attempts
// resetCount : When retryCount reaches a resetCount deletes all data stored in the pen and is the default.
void PenCommV1Callbacks.onPenPasswordRequest( IPenComm sender, int retryCount, int resetCount )
{
	// Input password to peer device
	mPenCommV1.ReqInputPassword( "string type password" );
}

// If your pen is not locked, or authentication is passed, it will be called.
// When it is called, You can use all function on your device.
void PenCommV1Callbacks.onPenAuthenticated( IPenComm sender )
{
}
```

**PenCommV2**

```cs
// It is occured when connection is established ( You cannot use function on your device without authentication )
// macAddress : identifier of device
// deviceName : name of device
// fwVersion : version of firmware
// protocolVersion : version of protocol
// subName : subname
// maxForce : maximum level of force sensor
void PenCommV2Callbacks.onConnected( IPenComm sender, string macAddress, string deviceName, string fwVersion, string protocolVersion, string subName, int maxForce )
{
}

// If your device is locked, it is called to input password.
// retryCount : Number of password verification attempts
// resetCount : When retryCount reaches a resetCount deletes all data stored in the pen and is the default.
void PenCommV2Callbacks.onPenPasswordRequest( IPenComm sender, int retryCount, int resetCount )
{
	// Input password to peer device
	mPenCommV2.ReqInputPassword( "string type password" );
}

// If your pen is not locked, or authentication is passed, it will be called.
// When it is called, You can use all function on your device.
void PenCommV1Callbacks.onPenAuthenticated( IPenComm sender )
{
}
```

#### Handling a handwriting data from peer device

```cs
// Identifier of note(paper) (it is consist of section and owner, note)
int section = 1;
int owner = 1;
int note = 102;

// Requests to set your note type.

// PenCommV1
mPenCommV1.ReqAddUsingNote(section, owner, note);

// PenCommV2
mPenCommV2.ReqAddUsingNote(section, owner, note);

// Callback method to receive writing data
// PenCommV1
void PenCommV1Callbacks.onReceiveDot( IPenComm sender, Dot dot )
{
// TODO : You should implements code using coordinate data.
}

// PenCommV2
void PenCommV2Callbacks.onReceiveDot( IPenComm sender, Dot dot, ImageProcessingInfo info )
{
// TODO : You should implements code using coordinate data.
}
```

#### Querying list of offline data in Smartpen's storage

```cs
// Requests to get offline data list
// PenCommV1
mPenCommV1.ReqOfflineDataList();

// PenCommV2
mPenCommV2.ReqOfflineDataList();

// Callback method to receive offline data list
// PenCommV1
void PenCommV1Callbacks.onOfflineDataList( IPenComm sender, OfflineDataInfo[] notes )
{
}

// PenCommV2
void PenCommV2Callbacks.onReceiveOfflineDataList( IPenComm sender, params OfflineDataInfo[] offlineNotes )
{
}

```

#### Downloading offline data in Smartpen's storage
**PenCommV1**
```cs
void PenCommV1Callbacks.onOfflineDataList( IPenComm sender, OfflineDataInfo[] notes )
{
}

// Requests to download offline data
mPenCommV1.ReqOfflineData( OfflineDataInfo data );

// it is invoked when begins downloading offline data
void PenCommV1Callbacks.onStartOfflineDownload( IPenComm sender )
{
}

// it is invoked when it changed progress ( it can be called several times )
void PenCommV1Callbacks.onUpdateOfflineDownload( IPenComm sender, int total, int progress )
{
}

// it is invoked when it obtained offline data ( it can be called several times )
void PenCommV1Callbacks.onReceiveOfflineStrokes( IPenComm sender, Stroke[] strokes )
{
}

// it is invoked when finished downloading
void PenCommV1Callbacks.onFinishedOfflineDownload( IPenComm sender, bool success )
{
}
```

**PenCommV2**
```cs
// Requests to download offline data
mPenCommV2.ReqOfflineData( data.Section, data.Owner, data.Note, false, data.Pages );

// it is invoked when begins downloading offline data
void PenCommV2Callbacks.onStartOfflineDownload( IPenComm sender )
{
}

// it is invoked when it obtained offline data ( it can be called several times )
void PenCommV2Callbacks.onReceiveOfflineStrokes( IPenComm sender, int total, int progress, Stroke[] strokes )
{
}

// it is invoked when finished downloading
void PenCommV2Callbacks.onFinishedOfflineDownload( IPenComm sender, bool success )
{
}
```
 
## LICENSE

Copyright©2017 by NeoLAB Convergence, Inc. All page content is property of NeoLAB Convergence Inc. <https://neolab.net> 

### GPL License v3 - for non-commercial purpose
    
For non-commercial use, follow the terms of the GNU General Public License. 

GPL License v3 is a free software: you can run, copy, redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation. 

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 

You should have received a copy of the GNU General Public License along with the program. If not, see <https://www.gnu.org/licenses/>. 


### 2. Commercial license - for commercial purpose 

For commercial use, it is not necessary or required to open up your source code. Technical support from NeoLAB Convergence, inc is available upon your request. 

Please contact our support team via email for the terms and conditions of this license. 

- Global: _global1@neolab.net
- Korea: _biz1@neolab.net

## Opensource Library

Please refer to the details of the open source library used below.

### 1. inthehand/32feet (https://github.com/inthehand/32feet/blob/master/LICENSE)

MIT License

Copyright (c) 2017 In The Hand Ltd

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

### 2. Ionic.Zip.Reduced (https://www.nuget.org/packages/DotNetZip.Reduced/)

Microsoft Public License (MS-PL)
This license governs use of the accompanying software. If you use the software, you
accept this license. If you do not accept the license, do not use the software.

1. Definitions
The terms "reproduce," "reproduction," "derivative works," and "distribution" have the
same meaning here as under U.S. copyright law.
A "contribution" is the original software, or any additions or changes to the software.
A "contributor" is any person that distributes its contribution under this license.
"Licensed patents" are a contributor's patent claims that read directly on its contribution.

2. Grant of Rights
(A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
(B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

3. Conditions and Limitations
(A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
(B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
(C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
(D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
(E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.

---
**Feel free to leave any comment or feedback [here](https://github.com/NeoSmartpen/Window-SDK2.0/issues)**
