// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

'use strict';

var HttpStatusCodes = require('./http_status_codes.js');
var Logger = require('./logger.js');
var request = require('request');

var RestfulApiHelper = function (hwProductKey, apiUri) {
    this._deviceId = hwProductKey.deviceId;
    this._password = hwProductKey.password;
    if (apiUri)
        this._sfAPIServiceBaseURI = apiUri;// 'https://sfapiservice.trafficmanager.net/'
    else
        this._sfAPIServiceBaseURI = 'https://msfapiservice.trafficmanager.net//';

    this._sfAPIServiceTokenEndPoint = this._sfAPIServiceBaseURI + 'token';
    this._sfAPIServiceDeviceAPIURI = this._sfAPIServiceBaseURI + 'device-api/device';
    this._currentDeviceToken = null;
    this.getAPIToken = function (done) {

        Logger.info('RestfulApiHelper - getAPIToken()');

        var content =
            {
                'grant_type': 'password',
                'email': this._deviceId,
                'password': this._password,
                'role': 'device'
            };
        var option = {
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            form: content,
            rejectUnauthorized: false
        }
        var uri = this._sfAPIServiceTokenEndPoint;

        request.post(uri, option, function (err, httpResponse, body) {
            if (err) {
                Logger.error('getAPIToken failed: ' + err);
                done(err)
            } else {

                if (httpResponse.statusCode === HttpStatusCodes.OK) {
                    done(null, JSON.parse(body));
                } else if (httpResponse.statusCode === HttpStatusCodes.BAD_REQUEST || httpResponse.statusCode === HttpStatusCodes.UNAUTHORIZED) {
                    Logger.error('getAPIToken Authentication Fail, statusCode=' + httpResponse.statusCode);
                    throw new Error('getAPIToken Authentication Fail, statusCode=' + httpResponse.statusCode);
                } else {
                    Logger.error('getAPIToken Error, statusCode=' + httpResponse.statusCode);
                    throw new Error('getAPIToken Error, statusCode=' + httpResponse.statusCode);
                }
            }
        });
    };

};

function getCurrentToken(deviceToken) {

    if (deviceToken && deviceToken['access_token']) {
        if (deviceToken['token_type'] === 'bearer')
            return deviceToken['access_token'];
        else {
            // x509 to do...
            Logger.error('Unsupported Device API Token Type ' + deviceToken['token_type']);
            return null;
        }

    } else
        return null;
};

RestfulApiHelper.prototype.getMessageSchemaMap = function (done) {

    Logger.info('RestfulApiHelper - getMessageSchemaMap()');

    // HTTP Request
    var endPointURI = this._sfAPIServiceDeviceAPIURI + '/' + this._deviceId + '/MessageSchema';
    var options = {
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        rejectUnauthorized: false
    }

    var token = getCurrentToken(this._currentDeviceToken);
    if (token != null) options.headers['Authorization'] = 'bearer ' + token;

    var self = this;

    request.get(endPointURI, options, function (err, httpResponse, body) {
        if (err) {
            Logger.error('err=' + err);
            throw new Error('err=' + err);
        } else {
            if (httpResponse.statusCode === HttpStatusCodes.OK) {
                //Logger.debug('body=%s', body);
                var messageSchemaList = JSON.parse(body);

                var messageSchemaMap = {};
                for (var index in messageSchemaList) {
                    messageSchemaMap[messageSchemaList[index].MessageCatalogId] = messageSchemaList[index].ElementList;

                    Logger.debug('-- Message schema loaded - MessageCatalogId = ' + messageSchemaList[index].MessageCatalogId);
                }

                if (done) done(null, messageSchemaMap);

            } else if (httpResponse.statusCode === HttpStatusCodes.BAD_REQUEST || httpResponse.statusCode === HttpStatusCodes.UNAUTHORIZED) {
                Logger.debug('getMessageSchemaMap BAD_REQUEST or UNAUTHORIZED');

                self.getAPIToken(function (err, deviceToken) {
                    if (err) {
                        Logger.error('err=' + err)
                    } else {
                        self._currentDeviceToken = deviceToken;
                        self.getMessageSchemaMap(done);
                    }
                });
            } else {
                Logger.error('getMessageSchemaMap error - httpResponse.statusCode=' + httpResponse.statusCode);
                throw new Error('getMessageSchemaMap err=' + httpResponse.statusCode);
            }
        }
    });
};

RestfulApiHelper.prototype.getDeviceModel = function (done) {

    Logger.info('RestfulApiHelper - getDeviceModel()');

    // HTTP Request
    var endPointURI = this._sfAPIServiceDeviceAPIURI + '/' + this._deviceId;
    var options = {
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        rejectUnauthorized: false
    }

    var token = getCurrentToken(this._currentDeviceToken);
    if (token != null) options.headers['Authorization'] = 'bearer ' + token;

    var self = this;

    request.get(endPointURI, options, function (err, httpResponse, body) {
        if (err) {
            Logger.error('err=' + err);
            throw new Error('err=' + err);
        } else {
            if (httpResponse.statusCode === HttpStatusCodes.OK) {
                //Logger.debug('body=%s', body);
                if (done) done(null, JSON.parse(body));

            } else if (httpResponse.statusCode === HttpStatusCodes.BAD_REQUEST || httpResponse.statusCode === HttpStatusCodes.UNAUTHORIZED) {
                Logger.debug('getDeviceModel BAD_REQUEST or UNAUTHORIZED');

                self.getAPIToken(function (err, deviceToken) {
                    if (err) {
                        Logger.error('err=' + err)
                    } else {
                        self._currentDeviceToken = deviceToken;
                        self.getDeviceModel(done);
                    }
                });
            } else {
                Logger.error('getDeviceModel error - httpResponse.statusCode=' + httpResponse.statusCode);
                throw new Error('getDeviceModel err=' + httpResponse.statusCode);
            }
        }

    });
};

module.exports = RestfulApiHelper;