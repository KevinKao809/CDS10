// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

var assert = require('assert');
var RestfulApiHelper = require('../lib/restful_api_helper.js');
var Logger = require('../lib/logger.js');

describe('Restful API Helper test', function () {

    var hwProductKey = {
        deviceId: 'nodedevice02',
        password: '1'
    };
    var messageCatalogIds = [45, 46];

    var apiUri = 'https://sfapiservice.trafficmanager.net/';// Optional

    var restfulApiHelper = new RestfulApiHelper(hwProductKey, apiUri);

    describe('getdevicemodel test 1', function () {

        it('should get deviceid without error', function (done) {
            restfulApiHelper.getDeviceModel(function (err, deviceModel) {
                if (err) done(error);
                else done();
            });
        });
    });

    describe('getdevicemodel test 2', function () {
        it('should get deviceid', function (done) {
            restfulApiHelper.getDeviceModel(function (err, deviceModel) {
                if (err) done(error);
                else {
                    assert.equal(deviceModel.deviceid, hwProductKey.deviceid);
                    done();
                }
            });
        });
    });

    describe('getmessageschemamap test 1', function () {
        it('should get message schema map without error', function (done) {
            restfulApiHelper.getMessageSchemaMap(function (err, messageSchemaMap) {
                if (err) done(error);
                else done();
            });
        })
    });

    describe('getMessageSchemaMap Test 2', function () {
        it('should get message schema map', function (done) {
            restfulApiHelper.getMessageSchemaMap(function (err, messageSchemaMap) {
                if (err) done(error);
                else {
                    var length = Object.keys(messageSchemaMap).length;
                    assert.equal(length, messageCatalogIds.length);

                    for (var i in messageCatalogIds) {
                        Logger.debug('messageCatalogIds[' + i + ']= ' + messageCatalogIds[i]);
                        assert.ok(messageSchemaMap.hasOwnProperty(messageCatalogIds[i]));
                    }

                    done();
                }
            });
        })
    });
})