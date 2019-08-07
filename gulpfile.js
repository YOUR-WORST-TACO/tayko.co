const gulp = require('gulp');
const sass = require('gulp-sass');
const del = require('del');
const rename = require('gulp-rename');

sass.compiler = require('node-sass');

gulp.task('sass', () => {
    gulp.src('Styles/**/*.scss')
        .pipe(sass({outputStyle: 'expanded'}).on('error', sass.logError))
        .pipe(gulp.dest('wwwroot/css/'));
    return gulp.src('Styles/**/*.scss')
        .pipe(sass({outputStyle: 'compressed'}).on('error', sass.logError))
        .pipe(rename('main.min.css'))
        .pipe(gulp.dest('wwwroot/css/'));
});

gulp.task('sass:clean', () => {
    return del([
        'css/main.css',
    ]);
});

gulp.task('sass:watch', () => {
    gulp.watch('Styles/**/*.scss', gulp.series('sass'));
});