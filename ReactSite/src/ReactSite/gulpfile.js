/// <binding AfterBuild='appDevBuild' />
"use strict";

/*
    Notes:
    Browserify walks the require/import tree to bundle all the dependencies into an in memory file.  Combined with reactify it can transpile .jsx into .js.
    Vinyl-source-stream along with vinyl-buffer used to convert the browserify output to a buffered vinyl (virtual file format) object.
*/

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    eslint = require("gulp-eslint"),
    source = require("vinyl-source-stream"),
    buffer = require("vinyl-buffer"),
    browserify = require("browserify");

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

paths.appPath = "JSX/app.jsx";
paths.appOutputName = "gulp_app.js";
paths.finalOutput = "wwwroot/build_gulp/";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean", ["clean:AppOutput", "clean:js", "clean:css"]);

gulp.task("min:js", function () {
    return gulp.src([paths.js, "!" + paths.minJs], { base: "." })
        .pipe(concat(paths.concatJsDest))
        .pipe(uglify())
        .pipe(gulp.dest("."));
});

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:js", "min:css"]);


//App build
gulp.task("setReactProductionMode", function () {
    process.env.NODE_ENV = 'production';
});

gulp.task("clean:AppOutput", function (cb) {   
    rimraf(paths.finalOutput, cb);
});

gulp.task("lint", ["clean"], function () {
    return gulp.src([paths.js, "JSX/**/*.jsx", "JSX/**/*.js"])
        .pipe(eslint())
        .pipe(eslint.format())
        .pipe(eslint.failAfterError());
});

gulp.task("browserify",["lint"], function () {
    return browserify(paths.appPath)        
        .transform("babelify", {presets: ["es2015", "react"]})
        .transform(require("browserify-css"))
        .bundle()
        .pipe(source(paths.appOutputName))
        .pipe(buffer())
        .pipe(uglify())
        .pipe(gulp.dest(paths.finalOutput));
});

gulp.task("appDevBuild", ["browserify"]);
gulp.task("appProdBuild", ["setReactProductionMode", "browserify"])
