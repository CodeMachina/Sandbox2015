'use strict';

var React = require('react');

var Default = React.createClass({
    render: function() {
        return (
          <div>
            <div>Boom react</div>
            <p>a bunch of other stuff written here</p>
            <section>
                <select>
                    <option value="1">Pizza</option>
                    <option value="2">Burger</option>
                </select>
            </section>
            <div>
                <label>Added an image to change the hash value</label>
                <div>
                    {/*<img src="http://pre00.deviantart.net/68b1/th/pre/f/2016/160/6/9/image_by_tyler0293-da5l2e1.jpg" />*/}
                    <img src="http://nomediakings.org/wp-content/uploads/2012/10/gy_poster_600x600.gif"/>
                </div>
            </div>
          </div>
        );
    }
});

module.exports = Default;