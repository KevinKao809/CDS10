// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

'use strict';

var colors = require('colors/safe');// npm install colors
var util = require('util');
//loglLevel 
//ALL : 1  
//Error and Debug : 2
//Error : 3
//None : >3
var logLevel = 3;
// set theme
colors.setTheme({
    silly: 'rainbow',
    input: 'blue',
    verbose: 'cyan',
    prompt: 'grey',
    info: 'grey',
    data: 'grey',
    help: 'cyan',
    warn: 'yellow',
    debug: 'green',
    error: 'red'
});

var TAG = '[Node SDK] ';
function loggerInfo(text) {
    if (logLevel <= 1)
        console.log(colors.info(TAG + text));
};

function loggerDebug(text) {
    if (logLevel <= 2)
        console.log(colors.debug(TAG + text));
};

function loggerError(text) {
    if (logLevel <= 3)
        console.log(colors.error(TAG + text));
};

module.exports = {
    info: loggerInfo,
    debug: loggerDebug,
    //warning: loggerWarning,
    error: loggerError
};