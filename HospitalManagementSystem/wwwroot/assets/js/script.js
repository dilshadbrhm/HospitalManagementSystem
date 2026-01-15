(function ($) {
	"use strict";


	// two Item Carousel
	if ($('.two-item').length) {
		$('.two-item').owlCarousel({
			loop: true,
			margin: 30,
			nav: true,
			singleItem: false,
			smartSpeed: 700,
			autoHeight: false,
			autoplay: true,
			autoplayTimeout: 10000,
			navText: ['<span class="fa fa-angle-left"></span>', '<span class="fa fa-angle-right"></span>'],
			responsive: {
				0: {
					items: 1
				},
				600: {
					items: 2
				},
				1024: {
					items: 2
				},
			}
		});
	}

	// three Item Carousel
	if ($('.three-item').length) {
		$('.three-item').owlCarousel({
			loop: true,
			margin: 30,
			nav: true,
			singleItem: false,
			smartSpeed: 700,
			autoHeight: false,
			autoplay: true,
			autoplayTimeout: 10000,
			navText: ['<span class="fa fa-angle-left"></span>', '<span class="fa fa-angle-right"></span>'],
			responsive: {
				0: {
					items: 1
				},
				600: {
					items: 2
				},
				1024: {
					items: 3
				},
			}
		});
	}
    
    
    

	$(document).on('ready', function () {
		$('.single-item').owlCarousel({
			loop: true,
			margin: 0,
			nav: true,
			dots: true,
			center: false,
			smartSpeed: 500,
			autoplay: true,
			autoplayTimeout: 5000,
			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],

			responsive: {
				0: {
					items: 1
				},
				600: {
					items: 1
				},
				800: {
					items: 1
				},
				1024: {
					items: 1
				},
				1600: {
					items: 1
				}
			}
		});
	});


	$(document).on('ready', function () {
		$('.four-item').owlCarousel({
			loop: true,
			margin: 0,
			nav: true,
			dots: true,
			center: false,
			smartSpeed: 500,
			autoplay: true,
			autoplayTimeout: 5000,
			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],

			responsive: {
				0: {
					items: 1
				},
				600: {
					items: 2
				},
				800: {
					items: 2
				},
				1024: {
					items: 3
				},
				1600: {
					items: 4
				}
			}
		});
	});

	$(document).on('ready', function () {
		$('.four-items').owlCarousel({
			loop: true,
			margin: 20,
			nav: true,
			dots: true,
			center: false,
			smartSpeed: 500,
			autoplay: true,
			autoplayTimeout: 5000,
			navText: ['<span class="fa fa-angle-left"></span>', '<span class="fa fa-angle-right"></span>'],

			responsive: {
				0: {
					items: 1
				},
				600: {
					items: 2
				},
				800: {
					items: 2
				},
				1024: {
					items: 3
				},
				1600: {
					items: 4
				}
			}
		});
	});


	$(document).on('ready', function () {
		$('.four-item-service').owlCarousel({
			loop: true,
			margin: 30,
			nav: false,
			dots: false,
			center: false,
			smartSpeed: 500,
			autoplay: true,
			autoplayTimeout: 5000,
			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],

			responsive: {
				0: {
					items: 1
				},
				600: {
					items: 2
				},
				800: {
					items: 3
				},
				1024: {
					items: 3
				},
				1600: {
					items: 4
				}
			}
		});
	});


	$(document).on('ready', function () {
		$('.four-item-service_type_two').owlCarousel({
			loop: true,
			margin: 0,
			nav: false,
			dots: false,
			center: false,
			smartSpeed: 500,
			autoplay: true,
			autoplayTimeout: 5000,
			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],

			responsive: {
				0: {
					items: 1
				},
				600: {
					items: 2
				},
				800: {
					items: 3
				},
				1024: {
					items: 3
				},
				1600: {
					items: 4
				}
			}
		});
	});

	$(document).on('ready', function () {
		$('.client-carousel').owlCarousel({
			loop: true,
			margin: 30,
			nav: false,
			dots: false,
			center: false,

			autoplay: false,

			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],

			responsive: {
				0: {
					items: 1
				},
				350: {
					items: 2
				},
				800: {
					items: 3
				},
				1024: {
					items: 4
				},
				1600: {
					items: 5
				}
			}
		});
	});

	//Main Slider Carousel
	if ($('.main-slider-one').length) {
		$('.main-slider-one').owlCarousel({
			loop: true,
			items: 1,
			margin: 0,
			nav: false,
			dots: false,
			animateOut: 'fadeOut',
			animateIn: 'fadeIn',
			active: true,
			smartSpeed: 1500,
			autoplayTimeout: 7000,
			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],
		});
	}

	//Main Slider Carousel
	if ($('.main-slider-two').length) {
		$('.main-slider-two').owlCarousel({
			loop: true,
			items: 1,
			margin: 0,
			nav: true,
			dots: false,
			animateOut: 'slideOutUp',
			animateIn: 'fadeIn',
			active: true,
			smartSpeed: 1500,
			autoplayTimeout: 7000,
			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],
		});
	}

	//Main Slider Carousel
	if ($('.main-slider-three').length) {
		$('.main-slider-three').owlCarousel({
			loop: true,
			items: 1,
			margin: 0,
			nav: false,
			dots: false,
			animateOut: 'slideOutUp',
			animateIn: 'fadeIn',
			active: true,
			smartSpeed: 1500,
			autoplayTimeout: 7000,
			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],
		});
	}

	//Main Slider Carousel
	if ($('.main-slider-four').length) {
		$('.main-slider-four').owlCarousel({
			loop: true,
			items: 1,
			margin: 0,
			nav: false,
			dots: true,
			animateOut: 'slideOutLeft',
			animateIn: 'fadeInRight',
			active: true,
			smartSpeed: 2500,
			autoplayTimeout: 7000,
			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],
		});
	}

	//Main Slider Carousel
	if ($('.main-slider-five').length) {
		$('.main-slider-five').owlCarousel({
			loop: true,
			items: 1,
			margin: 0,
			nav: false,
			dots: false,
			animateOut: 'fadeOut',
			animateIn: 'fadeIn',
			active: true,
			smartSpeed: 2500,
			autoplayTimeout: 7000,
			navText: ['<span class="fa flaticon-back"></span>', '<span class="fa flaticon-next"></span>'],
		});
	}


	/*----------------backtotop-----------------*/

	$(document).on('ready', function () {
		$(window).on('scroll', function () {
			if ($(this).scrollTop() > 100) {
				$('#scroll').fadeIn();
			} else {
				$('#scroll').fadeOut();
			}
		});
		$('#scroll').on('click', function () {
			$("html, body").animate({
				scrollTop: 0
			}, 600);
			return false;
		});
	});
	/*----------------backtotop-----------------*/

	/*--------------select-menu--------------*/
	$(function () {
		$("#language").selectmenu();

		$("#files").selectmenu();
		$("#country").selectmenu();
		$("#countrytwo").selectmenu();
		$("#department").selectmenu();
	});

	/* ==========================================================================
	   datepicker
	   ========================================================================== */
	$(function () {
		$("#datepicker").datepicker();
	});
	$(function () {
		$("#datepickerbook").datepicker();
	});

	/*-----price-filter--*/
	// slider-range
	if ($('#slider-range').length) {
		$(function () {
			$("#slider-range").slider({
				range: true,
				min: 0,
				max: 500,
				values: [75, 300],
				slide: function (event, ui) {
					$("#amount").val("$" + ui.values[0] + " - $" + ui.values[1]);
				}
			});
			$("#amount").val("$" + $("#slider-range").slider("values", 0) +
				" - $" + $("#slider-range").slider("values", 1));
		});
	}


	/*-----------------cart---product---increasing--------------*/


	$('.add').on('click', function () {
		if ($(this).prev().val() < 20) {
			$(this).prev().val(+$(this).prev().val() + 1);
		}
	});
	$('.sub').on('click', function () {
		if ($(this).next().val() > 1) {
			if ($(this).next().val() > 1) $(this).next().val(+$(this).next().val() - 1);
		}
	});


	/*-----------------preloader-----------------*/


	$(window).on('load', function () { // makes sure the whole site is loaded 
		$('.preloader').fadeOut(); // will first fade out the loading animation 
		$('.preloader').delay(500).fadeOut('slow'); // will fade out the white DIV that covers the website. 
		$('body').delay(500);
	})


})(window.jQuery);