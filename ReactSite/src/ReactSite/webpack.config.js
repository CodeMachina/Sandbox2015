var path = require('path');
var merge = require('webpack-merge');
var webpack = require('webpack');
var HtmlWebpackPlugin = require('html-webpack-plugin');
var CleanWebpackPlugin = require('clean-webpack-plugin');

var _PATH = path.resolve(__dirname); /* global __dirname */
var PATHS = {
    app: path.resolve(_PATH, 'JSX'),
    css: path.resolve(_PATH, 'wwwroot/css'),
    less: path.resolve(_PATH, 'wwwroot/less'),
    js: path.resolve(_PATH, 'wwwroot/js'),
    build: path.resolve(_PATH, 'wwwroot/build_webpack'),
    buildTemplates: path.resolve(_PATH, 'buildTemplates/webpack'),
    homeOutput: path.resolve(_PATH, 'Pages/Home')
};

var TARGET = process.env.npm_lifecycle_event;
process.env.BABEL_ENV = TARGET;

var commonConfig = {
    entry: {
        app: path.join(PATHS.app, 'app.jsx')
    },
    output: {
        path: PATHS.build,
        filename: 'webpack_app-[hash].js'
    },
    module: {
        loaders: [
            //{
            //    test: /\.css$/,
            //    loaders: ['style', 'css'],
            //    include: [PATHS.app, PATHS.css]
            //},
            {
                test: /\.less$/,
                loaders: ['style', 'css', 'less'],
                include: [PATHS.app, PATHS.less]
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
    },
    plugins: [
        new webpack.optimize.DedupePlugin(),
        new webpack.optimize.UglifyJsPlugin({
            compress: {
                warnings: false
            },
            output: {
                comments: false,
                semicolons: true
            }
        }),
        new HtmlWebpackPlugin({
            filename: PATHS.build + "/index.html",
            template: PATHS.buildTemplates + '/indextemplate.html',
            title: 'React Test Site',
            inject: false
        }),
        new HtmlWebpackPlugin({
            filename: PATHS.homeOutput + "/Home.cshtml",
            template: PATHS.buildTemplates + '/HomeTemplate.cshtml',
            title: 'React Test Site',
            inject: false,
            iisAppRoot: 'wwwroot/'
        }),
        new CleanWebpackPlugin(PATHS.build)
    ]
};

if (TARGET === 'start' || !TARGET) {
    module.exports = merge(commonConfig, {
        devServer: {
            contentBase: PATHS.build,
            historyApiFallback: true,
            hot: true,
            inline: true,
            progress: true
        },
        devtool: 'eval-source-map',
        plugins: [
            new webpack.HotModuleReplacementPlugin()
        ]
    });
}

if (TARGET === 'buildapp') {
    module.exports = merge(commonConfig, {
        output: {
            path: PATHS.build,
            filename: 'webpack_app-[chunkhash].js'
        },
    });
}

if (TARGET === 'build') {
    module.exports = merge(commonConfig, {});
}
