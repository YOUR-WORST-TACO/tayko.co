// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(function() {
    'use strict';
    let $page = $('#main'),
        options = {
            debug: true,
            prefetch: true,
            cacheLength: 4,
            onBefore: function($currentTarget, $container) {
                
                },
            onReady: {
                duration:1,
                render: function ($container, $newContent) {
                    $container.html($newContent);
                }
            },
            onAfter: function($container, $newContent) {
                document.querySelectorAll('pre code').forEach((block) => {
                    hljs.highlightBlock(block);
                });
            }
        },
        smoothState = $page.smoothState(options).data('smoothState');
})