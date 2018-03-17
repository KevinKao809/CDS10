// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

'use strict';

var Logger = require('./logger.js');
var Message = require('azure-iot-device').Message;
const SF_LASTUPDATED_TIMESTAMP = 'SF_LastUpdatedTimestamp';
const SF_SYSTEM_CONFIG = "SF_SystemConfig";
const SF_CUSTOMIZED_CONFIG = 'SF_CustomizedConfig';
var _twinDesiredVersion = 0;

var IoTHubTwinHelper = function (deviceClient) {
    this._deviceClient = deviceClient;
}

function getSfCustomConfig(properties) {
    if (properties == null)
        throw new ReferenceError('properties is \'' + properties + '\'');

    if (!properties[SF_CUSTOMIZED_CONFIG])
        return null;
    else
        return properties[SF_CUSTOMIZED_CONFIG];
}

/* Update Reported Properties of Twin */
function updateReportedProperties(deviceClient, patch, done) {

    getDeviceTwin(deviceClient, function (err, twin) {
        if (err) throw err;

        twin.properties.reported.update(patch, function (err) {
            if (err) {
                if (done) done(err);
            } else {
                if (done) done();
            }
        });
    });
}

function createReportedSystemPropertiesWithTimestamp(patch) {

    var reportedSystemProperties = {
        SF_LastUpdatedTimestamp: parseInt(new Date().getTime() / 1000),
        SF_SystemConfig: patch
    };

    return reportedSystemProperties;
}

function createReportedCustomPropertiesWithTimestamp(patch) {

    var reportedCustomProperties = {
        SF_LastUpdatedTimestamp: parseInt(new Date().getTime() / 1000),
        SF_CustomizedConfig: patch
    };

    return reportedCustomProperties;
}

/*Send D2C Message to Service Bus */
function sendD2CReportedPropertiesUpdate(deviceClient, reportedProperties, done) {
    if (!deviceClient) {
        throw new Error('device client is \'' + deviceClient + '\'');
    }

    var message = new Message(JSON.stringify(reportedProperties));
    message.properties.add('Type', 'Command');
    deviceClient.sendEvent(message, function (err) {
        if (err) {
            done(err);
        } else {
            done();
        }
    });
}

function getDeviceTwin(deviceClient, done) {
    if (!deviceClient) {
        throw new Error('device client is \'' + deviceClient + '\'');
    }

    deviceClient.getTwin(function (err, twin) {
        if (err) {
            done(err);
        } else {
            done(null, twin);
        }
    });
}

IoTHubTwinHelper.prototype.getTwin = function (done) {

    getDeviceTwin(this._deviceClient, done);
}

IoTHubTwinHelper.prototype.getDesiredCustomProperties = function (done) {

    getDeviceTwin(this._deviceClient, function (err, twin) {
        if (err) {
            done(err);
        } else {

            var sfCustomConfig = getSfCustomConfig(twin.properties.desired);
            done(null, sfCustomConfig);
        }
    });
}

IoTHubTwinHelper.prototype.getReportedCustomProperties = function (done) {

    getDeviceTwin(this._deviceClient, function (err, twin) {
        if (err) {
            done(err);
        } else {

            var sfCustomConfig = getSfCustomConfig(twin.properties.reported);
            done(null, sfCustomConfig);
        }
    });
}

IoTHubTwinHelper.prototype.updateReportedSystemProperties = function (patch, done) {

    var reportedSystemProperties = createReportedSystemPropertiesWithTimestamp(patch);

    Logger.info('[System] ReportedProperties=' + JSON.stringify(reportedSystemProperties));

    var deviceClient = this._deviceClient;

    updateReportedProperties(deviceClient, reportedSystemProperties, function (err) {
        if (err) {
            if (done) done(err);
        } else {
            var d2cCommand = {
                SF_LastUpdatedTimestamp: reportedSystemProperties[SF_LASTUPDATED_TIMESTAMP],
                SF_UpdatedType: SF_SYSTEM_CONFIG
            };
            sendD2CReportedPropertiesUpdate(deviceClient, d2cCommand, function (err) {
                if (err) {
                    if (done) done(err);
                } else
                    if (done) done();
            });
        }
    });
}

IoTHubTwinHelper.prototype.updateReportedCustomProperties = function (patch, done) {

    var reportedCustomProperties = createReportedCustomPropertiesWithTimestamp(patch);

    Logger.info('[Custom] ReportedProperties=' + JSON.stringify(reportedCustomProperties));

    var deviceClient = this._deviceClient;
    updateReportedProperties(deviceClient, reportedCustomProperties, function (err) {
        if (err) {
            if (done) done(err);
        } else {
            var d2cCommand = {
                SF_LastUpdatedTimestamp: reportedCustomProperties[SF_LASTUPDATED_TIMESTAMP],
                SF_UpdatedType: SF_CUSTOMIZED_CONFIG
            };
            sendD2CReportedPropertiesUpdate(deviceClient, d2cCommand, function (err) {
                if (err) {
                    if (done) done(err);
                } else
                    if (done) done();
            });
        }
    });
}

IoTHubTwinHelper.prototype.onDesiredPropertiesChanged = function (systemCallback, customCallback) {

    getDeviceTwin(this._deviceClient, function (err, twin) {
        if (err) throw new Error("could not get Twin - onDesiredPropertiesChanged");

        twin.on('properties.desired', function (delta) {

            var currentVer = _twinDesiredVersion;
            var newVer = delta['$version'];
            Logger.info('currentVer=' + currentVer + ', newVer=' + newVer);

            if (currentVer != newVer) {

                _twinDesiredVersion = newVer;

                if (delta[SF_SYSTEM_CONFIG])
                    systemCallback(delta[SF_SYSTEM_CONFIG]);

                if (delta[SF_CUSTOMIZED_CONFIG])
                    customCallback(delta[SF_CUSTOMIZED_CONFIG]);
            }
        });
    });
}

module.exports = IoTHubTwinHelper;