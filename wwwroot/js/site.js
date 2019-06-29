// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

Rainbow.defer = false;

$(function() {
    'use strict';
    var $page = $('#main'),
        options = {
            debug: true,
            prefetch: true,
            cacheLength: 4,
            onBefore: function($currentTarget, $container) {
                Rainbow.defer = true;
                },
            onReady: {
                duration:1,
                render: function ($container, $newContent) {
                    //Rainbow.color();
                    $container.html($newContent);
                    Rainbow.remove('csharp');
                    Rainbow.color();
                }
            }
        },
        smoothState = $page.smoothState(options).data('smoothState');
})