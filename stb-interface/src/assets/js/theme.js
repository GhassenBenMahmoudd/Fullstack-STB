/**
 * Cavistar | Multipurpose Responsive Template
 * Theme core scripts
 * 
 * @author Chitrakoot Web
 * @version 2.0
 */

    /* ----------------------------------

    JS Active Code Index
            
        01. Preloader
        02. Navigation
        03. mySVGsToInject
        04. Scroll To Top
        05. Video
        06. Menu Selector
        07. Copy to clipboard
        08. Sliders
        09. Tabs
        10. CountUp
        11. Popover
        12. Tooltips
        13. Countdown
        14. Redirection
            
    ---------------------------------- */    

(function($) {

    "use strict";

    var $window = $(window);


        /*------------------------------------
            01. Preloader
        --------------------------------------*/

        $('#preloader').fadeOut('normall', function () {
            $(this).remove();
        });


        /*------------------------------------
            02. Navigation
        --------------------------------------*/
      
          /* multi level dropdowns */
          $("ul.dropdown-menu [data-toggle='dropdown']").on("click", function(event) {
                event.preventDefault();
                event.stopPropagation();

                $(this).siblings().toggleClass("show");

                if (!$(this).next().hasClass('show')) {
                  $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
                }
                $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function(e) {
                  $('.dropdown-submenu .show').removeClass("show");
                });

          });

        /* dropdowns submenu */
          $('.dropdown-submenu > a').on("click", function(e) {
                var submenu = $(this);
                $('.dropdown-submenu .dropdown-menu').removeClass('show');
                submenu.next('.dropdown-menu').toggle();
                submenu.parents('.dropdown-submenu').siblings('li').children('.dropdown-menu').hide();

                submenu.toggleClass('active');
                submenu.parents('.dropdown-submenu').siblings('li').children('.dropdown-item').removeClass('active');

                e.stopPropagation();
                
            });


          /*------------------------------------
            03. mySVGsToInject
        --------------------------------------*/

          if ($(".feather,.svg-injector").length !== 0) {
          
            // Elements to inject
              var mySVGsToInject = document.querySelectorAll('img.feather, img.svg-injector');

              // Do the injection
              SVGInjector(mySVGsToInject);
          }
       

        /*------------------------------------
            04. Scroll To Top
        --------------------------------------*/

        $window.on('scroll', function() {
            if ($(this).scrollTop() > 500) {
                $(".scroll-to-top").fadeIn(400);

            } else {
                $(".scroll-to-top").fadeOut(400);
            }
        });

        $(".scroll-to-top").on('click', function(event) {
            event.preventDefault();
            $("html, body").animate({
                scrollTop: 0
            }, 600);
        });

       
        /*------------------------------------
            05. Video
        --------------------------------------*/

        if ($(".story-video").length !== 0) {
                $('.story-video').magnificPopup({
                delegate: '.video',
                type: 'iframe'
            });    
        }
        

    // === when document ready === //
    $(document).on("ready", function() {

        /*------------------------------------
            06. Menu Selector
        --------------------------------------*/

        var urlparam = window.location.pathname.split('/');
        var menuselctor = window.location.pathname;
        if (urlparam[urlparam.length - 1].length > 0) menuselctor = urlparam[urlparam.length - 1];
        else menuselctor = urlparam[urlparam.length - 2];
        $('.navbar-nav li').find('a[href="' + menuselctor + '"]').closest('li').addClass('active').parents().eq(1).addClass('current');
        $('.navbar-nav ul.dropdown-menu li').find('a[href="' + menuselctor + '"]').closest('li').addClass('active').parents().eq(3).addClass('current');
        $('.navbar-nav li ul li').find('a[href="' + menuselctor + '"]').parents().eq(7).addClass('current');


        /*------------------------------------
            07. Copy to clipboard
        --------------------------------------*/

        if ($(".copy-clipboard").length !== 0) {
            new ClipboardJS('.copy-clipboard');
            $('.copy-clipboard').on('click', function() {
                var $this = $(this);
                var originalText = $this.text();
                $this.text('Copied');
                setTimeout(function() {
                    $this.text('Copy')
                    }, 2000);
            });
        };        


        /*------------------------------------
            08. Sliders
        --------------------------------------*/

        if ($(".owl-carousel").length !== 0) {
            $('.classic-slider').owlCarousel({
                items: 1,
                loop: true,
                margin: 0,
                dots: true,
                nav: false,
                autoplay: true,
                smartSpeed: 1200,
                responsive: {
                    0: {
                        dots: false
                    },
                    576: {
                        items: 1
                    }
                }
            });
            $('.main-slider .owl-carousel').owlCarousel({
                items: 1,
                loop: true,
                margin: 0,
                dots: false,
                nav: true,
                autoplay: true,
                smartSpeed: 1200,
                navText: ["<i class='fas fa-arrow-left'></i>", "<i class='fas fa-arrow-right'></i>"]
            });
            $('.case-study.owl-carousel').owlCarousel({
                items: 1,
                loop: true,
                dots: false,
                nav: false,
                autoplay: true,
                smartSpeed: 1200,
                margin: 30,
                responsive: {
                    0: {
                        items: 1
                    },
                    576: {
                        items: 2
                    },
                    992: {
                        items: 3
                    },
                    1200: {
                        items: 4
                    }
                }
            });
            $('.testimonial.owl-carousel').owlCarousel({
                items: 1,
                loop: true,
                margin: 0,
                dots: true,
                nav: false,
                autoplay: true,
                smartSpeed: 1200,
                navText: ["<i class='fas fa-arrow-left'></i>", "<i class='fas fa-arrow-right'></i>"],
                responsive: {
                    0: {
                        items: 1
                    },
                    600: {
                        items: 1
                    },
                    1000: {
                        items: 1
                    }
                }
            });
            $('.testimonial-style-two .owl-carousel').owlCarousel({
                items: 1,
                loop: true,
                margin: 0,
                dots: false,
                nav: false,
                autoplay: true,
                smartSpeed: 1200,
            });
        };

        /*------------------------------------
            09. Tabs
        --------------------------------------*/

        //Horizontal Tab
        if ($(".horizontaltab").length !== 0) {
            $('.horizontaltab').easyResponsiveTabs({
                type: 'default', //Types: default, vertical, accordion
                width: 'auto', //auto or any width like 600px
                fit: true, // 100% fit in a container
                tabidentify: 'hor_1', // The tab groups identifier
                activate: function(event) { // Callback function if tab is switched
                    var $tab = $(this);
                    var $info = $('#nested-tabInfo');
                    var $name = $('span', $info);
                    $name.text($tab.text());
                    $info.show();
                }
            });
        }

        // Child Tab
        if ($(".childverticaltab").length !== 0) {
            $('.childverticaltab').easyResponsiveTabs({
                type: 'vertical',
                width: 'auto',
                fit: true,
                tabidentify: 'ver_1', // The tab groups identifier
                activetab_bg: '#fff', // background color for active tabs in this group
                inactive_bg: '#F5F5F5', // background color for inactive tabs in this group
                active_border_color: '#c1c1c1', // border color for active tabs heads in this group
                active_content_border_color: '#c1c1c1' // border color for active tabs contect in this group so that it matches the tab head border
            });
        }

        //Vertical Tab
        if ($(".verticaltab").length !== 0) {
            $('.verticaltab').easyResponsiveTabs({
                type: 'vertical', //Types: default, vertical, accordion
                width: 'auto', //auto or any width like 600px
                fit: true, // 100% fit in a container
                closed: 'accordion', // Start closed if in accordion view
                tabidentify: 'hor_1', // The tab groups identifier
                activate: function(event) { // Callback function if tab is switched
                    var $tab = $(this);
                    var $info = $('#nested-tabInfo2');
                    var $name = $('span', $info);
                    $name.text($tab.text());
                    $info.show();
                }
            });
        }

        /*------------------------------------
            10. CountUp
        --------------------------------------*/

        if ($(".countup").length !== 0) {
            $('.countup').counterUp({
            delay: 25,
            time: 2000
        });
        }
        
        /*------------------------------------
            11. Popover
        --------------------------------------*/

        $(function () {
            $('[data-toggle="popover"]').popover()
        })


        /*------------------------------------
            12. Tooltips
        --------------------------------------*/

        $(function () {
            $('[data-toggle="tooltip"]').tooltip()
        })


        /*------------------------------------
            13. Countdown
        --------------------------------------*/
       
        if ($(".countdown").length !== 0) {
            $(".countdown").countdown({
                date: "01 Jan 2021 00:01:00", //set your date and time. EX: 15 May 2014 12:00:00
                format: "on"
            });
        }

        /*------------------------------------
            14. Redirection
        --------------------------------------*/

        $(".sidenav-right a[href^='#']").click(function(e) {
                e.preventDefault();

                var position = $($(this).attr("href")).offset().top - 120;

                $("body, html").animate({
                    scrollTop: position
                }, 500);
        });
      
    });

 

})(jQuery);

