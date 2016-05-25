'use strict';

//require('../wwwroot/css/main.css');
require('../wwwroot/less/main.less');
require('../wwwroot/js/test.js');

var ReactDOM = require('react-dom');
var React = require('react');
var Default = require('./Default.jsx');

ReactDOM.render(<Default />, document.getElementById('myApp'));