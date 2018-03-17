// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

'use strict';

var EventEmitter = require('events').EventEmitter;
var util = require('util');
var fs = require('fs');
var Logger = require('./lib/logger.js');
var RestfulApiHelper = require('./lib/restful_api_helper.js');
var IoTHubTwinHelper = require('./lib/iothub_twin_helper.js');
var request = require('request');
var Client = require('azure-iot-device').Client;
var Message = require('azure-iot-device').Message;
var path = require('path');
const sdkVersion = '1.0.1';
var _sfDeviceClient;
var _onDesiredCustomPropertiesChanged = null;

Logger.info('Microsoft Connected Device Studio Node.js Device SDK [Version: ' + sdkVersion + ']');

function safeCallback(callback, error, result) {
    if (callback) callback(error, result);
}

function verifyAndAppendIoTHubAuthCertificatePath(deviceModel, certificatePath) {
    if (deviceModel.IoTHubAuthenticationType.toLowerCase() === 'certificate') {
        if (certificatePath) {
            deviceModel.CertificateFileName = certificatePath + deviceModel.CertificateFileName;
        }
        else {
            throw new ReferenceError('password cannot be \'' + certificatePath + '\'');
        }
    }

    Logger.debug('certificatePath=' + certificatePath);
    Logger.debug('CertificateFileName=' + deviceModel.CertificateFileName);
}

function getTransportType(protocol) {

    switch (protocol) {
        case 'http':
            return require('azure-iot-device-http').Http;
        case 'amqp':
            return require('azure-iot-device-amqp').Amqp;
        case 'mqtt':
            return require('azure-iot-device-mqtt').Mqtt;
        default:
            Logger.error('Protocol ' + protocol + ' does not support!');
            throw ReferenceError('Protocol ' + protocol + ' does not support!');
    }
}

function getConnectionString(authType, deviceModel) {
    if (authType === 'key') {
        return 'HostName=' + deviceModel.IoTHubName +
            ';DeviceId=' + deviceModel.DeviceId + ';SharedAccessKey=' + deviceModel.DeviceKey;

    } else if (authType === 'certificate') {
        return 'HostName=' + deviceModel.IoTHubName +
            ';DeviceId=' + deviceModel.DeviceId + ';x509=true';
    } else {
        Logger.error('authType ' + authType + ' does not support!');
        throw ReferenceError('authType ' + authType + ' does not support!');
    }
}

function getAzureDeviceClient(authType, deviceModel, connectionString, protocol) {
    if (authType === 'key') {

        return Client.fromConnectionString(connectionString, protocol);

    } else if (authType === 'certificate') {

        var client = Client.fromConnectionString(connectionString, protocol);
        Logger.debug('[readFileSync]=' + fs.readFileSync(deviceModel.CertificateFileName, 'utf-8').toString());
        Logger.debug('[CertificatePassword]=' + deviceModel.CertificatePassword);
        var options = {
            cert: fs.readFileSync(deviceModel.CertificateFileName, 'utf-8').toString(),
            key: fs.readFileSync(deviceModel.CertificateFileName, 'utf-8').toString(),
            passphrase: deviceModel.CertificatePassword
        };
        client.setOptions(options);

        return client;
    } else {
        throw new ReferenceError('IoTHubAuthenticationType cannot be \'' + authType + '\'');
    }
}

function getAzureIoTDeviceSDKClient(deviceModel) {

    var authType = deviceModel.IoTHubAuthenticationType.toLowerCase();
    var connectionString = getConnectionString(authType, deviceModel);
    Logger.info('connstr:' + connectionString);

    return getAzureDeviceClient(authType, deviceModel, connectionString, getTransportType(deviceModel.IoTHubProtocol.toLowerCase()));
}

function onDesiredSystemPropertiesChanged(delta) {

    if (!delta) return;

    Logger.debug('---- onDesiredSystemPropertiesChanged ----');
    Object.keys(delta).forEach(function (key) {

        Logger.debug(key + ': ' + delta[key]);

        // do something here...


    });
    Logger.debug('------------------------------------------');

    // Update Reported System Properties
    _sfDeviceClient._iothubTwinHelper.updateReportedSystemProperties(delta, function (err) {
        if (err) Logger.error('Could not update the Reported System Properties');
    });
}

function onDesiredCustomPropertiesChanged(delta) {

    if (_onDesiredCustomPropertiesChanged)
        _onDesiredCustomPropertiesChanged(delta);
}

var SfDeviceClient = function (hwProductKey, certificatePath, apiUri) {
    EventEmitter.call(this);

    _sfDeviceClient = this;
    this._hwProductKey = hwProductKey;
    this._certificatePath = certificatePath;
    this._deviceModel = null;
    this._messageSchemaMap = {};
    this._client = null;
    this._restfulApiHelper = new RestfulApiHelper(hwProductKey, apiUri);
    this._iothubTwinHelper = null;
    this._onDesiredCustomPropertiesChanged = null;

    function testDisconnectAndError(sfDeviceClient, msg) {
        var encodedString = String.fromCharCode.apply(null, msg.data),
            decodedString = decodeURIComponent(escape(encodedString));

        if (decodedString === 'disconnect') {
            sfDeviceClient._client.complete(msg, function printResult(err, res) {
                if (err) Logger.error(' error: ' + err.toString());
                if (res) Logger.debug(' status: ' + res.constructor.name);
            });
            sfDeviceClient._client.removeAllListeners();// remove all listeners
            Logger.error('removeAllListeners');
            Logger.error('disconnect');
            sfDeviceClient.emit('disconnect');

        } else if (decodedString === 'error') {
            sfDeviceClient._client.complete(msg, function printResult(err, res) {
                if (err) Logger.error(' error: ' + err.toString());
                if (res) Logger.debug(' status: ' + res.constructor.name);
            });

            var err = {};
            err['message'] = 'error test!';

            sfDeviceClient.emit('error', err);
            Logger.error(err.message);
        } else
            sfDeviceClient.emit('message', msg);
    };

    this.getAzureIoTDeviceSDKClientCallback = function (deviceModel, done) {

        this._client = getAzureIoTDeviceSDKClient(deviceModel);

        Logger.debug('client open');

        // Client Open
        this._client.open(function (err) {
            if (err) {
                Logger.error('Could not connect: ' + err.message);
                if (done) done(err);
            } else {
                Logger.debug('client connected');

                _sfDeviceClient._iothubTwinHelper = new IoTHubTwinHelper(_sfDeviceClient._client);
                _sfDeviceClient._iothubTwinHelper.onDesiredPropertiesChanged(onDesiredSystemPropertiesChanged, onDesiredCustomPropertiesChanged);

                _sfDeviceClient._client.on('message', function (msg) {
                    //testDisconnectAndError(thisSfDeviceClient, msg);
                    _sfDeviceClient.emit('message', msg);
                });

                _sfDeviceClient._client.on('error', function (err) {
                    _sfDeviceClient.emit('error', err);
                    Logger.error(err.message);
                });

                _sfDeviceClient._client.on('disconnect', function () {
                    _sfDeviceClient._client.removeAllListeners();// remove all listeners
                    Logger.debug('removeAllListeners');
                    _sfDeviceClient.emit('disconnect');
                    Logger.debug('disconnect');
                });
            }

            if (done) done();
        });
    };

    this.getDeviceModelCallback = function (err, deviceModel, done) {
        if (err)
            throw new Error('Unable to get device Model - err=' + err);
        else
            this._deviceModel = deviceModel;

        verifyAndAppendIoTHubAuthCertificatePath(this._deviceModel, this._certificatePath);

        this.getAzureIoTDeviceSDKClientCallback(this._deviceModel, done);
    };

    this.getMessageSchemaMapCallback = function (err, messageSchemaMap, done) {
        if (err)
            throw new Error('Unable to get message schema map - err=' + err);
        else
            this._messageSchemaMap = messageSchemaMap;

        this._restfulApiHelper.getDeviceModel(function (err, deviceModel) {
            _sfDeviceClient.getDeviceModelCallback(err, deviceModel, done);
        });
    };

    this.initial = function (openCallback) {

        Logger.info('SfDeviceClient - initial');

        this._restfulApiHelper.getMessageSchemaMap(function (err, messageSchemaMap) {
            _sfDeviceClient.getMessageSchemaMapCallback(err, messageSchemaMap, openCallback);
        });
    }
};

util.inherits(SfDeviceClient, EventEmitter);

SfDeviceClient.createSfDeviceClient = function createSfDeviceClient(deviceId, password, certificatePath, apiUri) {

    if (!deviceId) throw new ReferenceError('deviceId cannot be \'' + deviceId + '\'');

    if (!password) throw new ReferenceError('password cannot be \'' + password + '\'');

    var hwProductKey = {
        deviceId: deviceId,
        password: password
    };

    Logger.debug('deviceId: ' + hwProductKey.deviceId + ', password: ' + hwProductKey.password);

    return new SfDeviceClient(hwProductKey, certificatePath, apiUri);
};

SfDeviceClient.prototype.open = function (openCallback) {

    Logger.info('SfDeviceClient - open');

    // initial    
    this.initial(openCallback);

};

SfDeviceClient.prototype.close = function (closeCallback) {
    Logger.info('SfDeviceClient - close');

    if (this._client)
        this._client.close(closeCallback);
};

SfDeviceClient.prototype.complete = function (message, completeCallback) {
    if (this._client)
        this._client.complete(message, completeCallback);
};

SfDeviceClient.prototype.sendEvent = function (messageCatalogId, message, sendEventCallback) {

    if (!message) {
        throw new ReferenceError('Message String cannot be \'' + messageString + '\'');
    }
    var deviceMessage = {};
    if (this._messageSchemaMap[messageCatalogId] == null) {
        throw new ReferenceError('Message CatalogId dose not existed : \'' + messageCatalogId + '\'');
    }
    for (var index in this._messageSchemaMap[messageCatalogId]) {
        var element = this._messageSchemaMap[messageCatalogId][index];
        if (element.MandatoryFlag) {
            if (message[element.Name] == null) {
                throw new ReferenceError('Element \'' + element.Name + '\' must be existed');
            }
            deviceMessage[element.Name] = message[element.Name];
        } else {
            if (message[element.Name] != null) {
                deviceMessage[element.Name] = message[element.Name];
            }
        }
    }

    var msg = new Message(JSON.stringify(deviceMessage));
    msg.properties.add('messageCatalogId', messageCatalogId);

    this._client.sendEvent(msg, sendEventCallback);
};

SfDeviceClient.prototype.uploadToBlob = function (filePath, blobName, done) {

    if (!filePath) {
        throw new ReferenceError('filePath cannot be \'' + filePath + '\'');
    }

    if (!blobName) {
        throw new ReferenceError('blobName cannot be \'' + blobName + '\'');
    }

    if (!this._client) {
        throw new ReferenceError('device client is \'' + this._client + '\'');
    }

    var self = this;
    fs.stat(filePath, function (err, fileStats) {

        var fileStream = fs.createReadStream(filePath);
        self._client.uploadToBlob(blobName, fileStream, fileStats.size, function (err, result) {

            if (err) {
                if (done) done(err);
            } else {
                if (done) done();
            }
            fileStream.destroy();
        });
    });
}

SfDeviceClient.prototype.getTwin = function (done) {

    if (!this._iothubTwinHelper)
        throw new Error('The Initial should be called before getTwin');

    this._iothubTwinHelper.getTwin(function (err, twin) {
        if (err) {
            if (done) done(err);
        } else {
            if (done) done(null, twin);
        }
    });

}

SfDeviceClient.prototype.getDesiredCustomProperties = function (done) {

    if (!this._iothubTwinHelper)
        throw new Error('The Initial should be called before getDesiredCustomProperties');

    this._iothubTwinHelper.getDesiredCustomProperties(function (err, desiredCustomProperties) {
        if (err) {
            if (done) done(err);
        } else {
            if (done) done(null, desiredCustomProperties);
        }
    });
}

SfDeviceClient.prototype.getReportedCustomProperties = function (done) {

    if (!this._iothubTwinHelper)
        throw new Error('The Initial should be called before getDesiredCustomProperties');

    this._iothubTwinHelper.getReportedCustomProperties(function (err, reportedCustomProperties) {
        if (err) {
            if (done) done(err);
        } else {
            if (done) done(null, reportedCustomProperties);
        }
    });
}

SfDeviceClient.prototype.updateReportedCustomProperties = function (patch, done) {

    if (!this._iothubTwinHelper)
        throw new Error('The Initial should be called before updateReportedCustomProperties');

    this._iothubTwinHelper.updateReportedCustomProperties(patch, function (err) {
        if (err) {
            if (done) done(err);
        } else {
            if (done) done();
        }
    });
}

SfDeviceClient.prototype.SetOnDesiredCustomPropertiesChanged = function (done) {
    _onDesiredCustomPropertiesChanged = done;
}

module.exports = SfDeviceClient;