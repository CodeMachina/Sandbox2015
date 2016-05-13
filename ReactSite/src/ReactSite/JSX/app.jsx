'use strict';

var maincss = require('../wwwroot/css/main.css');
var testjs = require('../wwwroot/js/test.js');

var ReactDOM = require('react-dom');
var React = require('react');
var Default = require('./Default.jsx');

ReactDOM.render(<Default />, document.getElementById('myApp'));