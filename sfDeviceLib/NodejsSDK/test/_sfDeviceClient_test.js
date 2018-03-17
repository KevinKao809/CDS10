// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

var assert = require('assert');
var SfDeviceClient = require('../sfDeviceClient.js');

describe('SmartFactory Client Test', function () {

    var deviceId = 'nodedevice02';
    var devicePassword = '1';
    var apiUri = 'https://sfapiservice.trafficmanager.net/';// Optional
    var certificatePath = 'C:\\temp\\';

    var sfDeviceClient = SfDeviceClient.createSfDeviceClient(deviceId, devicePassword, certificatePath, apiUri);

    describe('sfDeviceClient Test 1', function () {
        it('should open/close without error', function (done) {
            sfDeviceClient.open(function (err) {
                if (err)
                    done(err);
                else {
                    sfDeviceClient.close(function (err) {
                        if (err)
                            done(err);
                        else
                            done();
                    });
                }
            });
        });

    });

    describe('sfDeviceClient Test 2', function () {
        it('should receive the Error property of listener', function (done) {
            sfDeviceClient.open(function (err) {
                if (err)
                    done(err);
                else {
                    var error = {};
                    error['message'] = 'error test!';

                    sfDeviceClient.on('error', function (err) {
                        console.error('err.message=' + err.message);
                        assert.equal(err.message, error['message']);
                        sfDeviceClient.removeAllListeners();
                        sfDeviceClient.close(function (err) {
                            if (err)
                                done(err);
                            else
                                done();
                        });
                    });

                    sfDeviceClient._client.emit('error', error);
                }
            });
        });
    });

});