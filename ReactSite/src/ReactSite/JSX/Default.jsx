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
          </div>
        );
    }
});

module.exports = Default;