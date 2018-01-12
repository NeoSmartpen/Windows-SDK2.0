# C# Software Development Kit for Neo Smartpen

## Requirements

1.  Windows 7 or later ( Not Windows Store Apps )
2.  Microsoft Visual Studio 2013 or later
3.  Microsoft .NET Framework 4 or later
4.  Standard Bluetooth Dongles ( Bluetooth Specification Version 2.1 + EDR or later with Microsoft Bluetooth stack )
5.  SDK packages for Neo Smartpen

## Dependencies

1. InTheHand.Net.Personal.dll ( v3.5 )
2. Ionic.Zip.Reduced.dll ( v1.9 )

## Supported Smartpen

- F110 (N2)
- F50, F120

## SDK Structure

| Namespace | Description |
|:---|:--------|
| Neosmartpen.Net | Framework providing interface for communication |
| Neosmartpen.Net.Bluetooth | Provide features for controlling Bluetooth communication |
| Neosmartpen.Net.Support | Provide features except communication ( such as graphics ) |
| Neosmartpen.Net.Protocol.v1 | Handling data and communication with peer device ( protocol version is 1.xx )   |
| Neosmartpen.Net.Protocol.v2 | Handling data and communication with peer device ( protocol version is 2.xx ) |

## Basic Usage

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

### Ncode Coodinate Description

+ **Dot.X, Dot.Y**
Coordinates of our NCode cell.( NCode's cell size is 2.371mm )

+ **Dot.Fx, Dot.Fy**
It is fractional part of NCode Coordinates. ( maximum value is 100 )

+ **How to get millimeter unit from NCode unit**
( Dot.X + Dot.Fx  x 0.01) x 2.371 = millimeter unit

## Give Feedback

Please reports bugs or issues to [link](https://github.com/NeoSmartpen/UWPSDK/issues)

## Ncode™ SERVICE DEVELOPMENT GETTING STARTED GUIDE

<< [https://github.com/NeoSmartpen/Documentations/blob/master/Ncode™ Service Development Getting Started Guide v1.01.pdf](https://github.com/NeoSmartpen/Documentations/blob/master/Ncode%E2%84%A2%20Service%20Development%20Getting%20Started%20Guide%20v1.01.pdf) >>
 
## LICENSE

NeoSmartpen SDK is Copyright (c) 2017 NeoLAB Convergence, Inc.

We provide two types of license for Pen SDK.

### 1. GPL license v3
    
This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. 
    
This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details. 
    
You should have received a copy of the GNU General Public License along with this program. 
    
If not, see <http://www.gnu.org/licenses/>.
    
*ps: normally, 3rd party developer can inquiry via the github issue column, but depending on the situation of internal, the answer may be delayed somewhat.



### 2. Commercial license

That does not require the source code open to be released, and technical support is available.

Please contact following to get more information:

- Global: _globiz@neolab.net
- Korea: _dombiz@neolab.net

