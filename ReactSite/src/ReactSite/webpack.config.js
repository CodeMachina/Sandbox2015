var path = require('path');
var merge = require('webpack-merge');
var webpack = require('webpack');

var _PATH = path.resolve(__dirname); /* global __dirname */
var PATHS = {
    app: path.resolve(_PATH, 'JSX'),
    css: path.resolve(_PATH, 'wwwroot/css'),
    js: path.resolve(_PATH, 'wwwroot/js'),
    build: path.resolve(_PATH, 'wwwroot/build_webpack')
};

var TARGET = process.env.npm_lifecycle_event;
process.env.BABEL_ENV = TARGET;

var commonConfig = {
    entry: {
        app: path.join(PATHS.app, 'app.jsx')
    },
    output: {
        path: PATHS.build,
        filename: 'webpack_app.js'
    },
    module: {
        loaders: [
            {
                test: /\.css$/,
                loaders: ['style', 'css'],
                include: [PATHS.app, PATHS.css]
            },
            {
                test: /\.jsx?$/,
                loader: 'babel-loader',
                query: {
                    presets: ['react', 'es2015'],
                    "env": {
                        "start": {
                            "presets": ["react-hmre"]
                        }
                    }
                },
                include: [PATHS.app]
            }
        ],
        preLoaders: [
            {
                test: /\.jsx?$/,
                loaders: ['eslint-loader'],
                include: [PATHS.app, PATHS.js]
            }
        ]
    },
    resolve: {
        extensions: ['', '.js', '.jsx']
    }
};

if (TARGET === 'start' || !TARGET) {
    module.exports = merge(commonConfig, {
        devServer: {
            contentBase: PATHS.build,
            historyApiFallback: true,
            hot: true,
            inline: true,
            progress: true,
            stats: 'errors-only',
            host: process.env.HOST,
            port: process.env.PORT
        },
        devtool: 'eval-source-map',
        plugins: [
            new webpack.HotModuleReplacementPlugin()
        ]
    });
}

if (TARGET === 'build') {
    module.exports = merge(commonConfig, {});
}
