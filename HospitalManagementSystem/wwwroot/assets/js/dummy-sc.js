(function($) {
	"use strict";


	
	   //Main Slider Carousel
		 if ($('.main-slider-one').length) {
			$('.main-slider-one').owlCarousel({
				loop:true,
				items: 1,
				margin: 0,
				nav: false,
				dots:false,
				animateOut: 'fadeOut',
			animateIn: 'fadeIn',
				active: true,
				smartSpeed: 1500,
				autoplay: 7000,
				navText: [ '<span class="fa fa-chevron-left"></span>', '<span class="fa fa-chevron-right"></span>' ]			
			});    		
		}

   //Main Slider Carousel
	 if ($('.main-slider-two').length) {
		$('.main-slider-two').owlCarousel({
			loop:true,
			items: 1,
			margin: 0,
			nav: true,
			dots:false,
			animateOut: 'slideOutUp',
			animateIn: 'fadeIn',
			active: true,
			smartSpeed: 1500,
			autoplay: 7000,
			navText: [ '<span class="fa fa-angle-left"></span>', '<span class="fa fa-angle-right"></span>' ]			
		});    		
	}

	    //Main Slider Carousel
		if ($('.main-slider-three').length) {
			$('.main-slider-three').owlCarousel({
				loop:true,
				items: 1,
				margin: 0,
				nav: false,
				dots:false,
				animateOut: 'slideOutUp',
				animateIn: 'fadeIn',
				active: true,
				smartSpeed: 1500,
				autoplay: 7000,
				navText: [ '<span class="fa fa-chevron-left"></span>', '<span class="fa fa-chevron-right"></span>' ]			
			});    		
		}

		    //Main Slider Carousel
				if ($('.main-slider-four').length) {
					$('.main-slider-four').owlCarousel({
						loop:true,
						items: 1,
						margin: 0,
						nav: false,
						dots:false,
						animateOut: 'slideOutLeft',
						animateIn: 'fadeInRight',
						active: true,
						smartSpeed: 2500,
						autoplay: 7000,
						navText: [ '<span class="fa fa-chevron-left"></span>', '<span class="fa fa-chevron-right"></span>' ]			
					});    		
				}

				  //Main Slider Carousel
					if ($('.main-slider-five').length) {
						$('.main-slider-five').owlCarousel({
							loop:true,
							items: 1,
							margin: 0,
							nav: false,
							dots:false,
							animateOut: 'fadeOut',
    		animateIn: 'fadeIn',
							active: true,
							smartSpeed: 2500,
							autoplay: 7000,
							navText: [ '<span class="fa fa-chevron-left"></span>', '<span class="fa fa-chevron-right"></span>' ]			
						});    		
					}

					


	
/*----------------backtotop-----------------*/

		$(document).ready(function(){ 
			$(window).scroll(function(){ 
					if ($(this).scrollTop() > 100) { 
							$('#scroll').fadeIn(); 
					} else { 
							$('#scroll').fadeOut(); 
					} 
			}); 
			$('#scroll').click(function(){ 
					$("html, body").animate({ scrollTop: 0 }, 600); 
					return false; 
			}); 
	});
/*----------------backtotop-----------------*/
	
/*--------------select-menu--------------*/
$( function() {
	$( "#language" ).selectmenu();

	$( "#files" ).selectmenu();
	$( "#country" ).selectmenu();
	$( "#countrytwo" ).selectmenu();
	$( "#department" ).selectmenu();
} );

/* ==========================================================================
   datepicker
   ========================================================================== */
   $(function() {
		$( "#datepicker" ).datepicker();
		});
		$( function() {
			$( "#datepickerbook" ).datepicker();
		} );
	
/*-----price-filter--*/
// slider-range
if($('#slider-range').length){
	$( function() {
	$( "#slider-range" ).slider({
		range: true,
		min: 0,
		max: 500,
		values: [ 75, 300 ],
		slide: function( event, ui ) {
		$( "#amount" ).val( "$" + ui.values[ 0 ] + " - $" + ui.values[ 1 ] );
		}
	});
	$( "#amount" ).val( "$" + $( "#slider-range" ).slider( "values", 0 ) +
		" - $" + $( "#slider-range" ).slider( "values", 1 ) );
	});	
 }


 /*-----------------cart---product---increasing--------------*/
 

 $('.add').click(function () {
	if ($(this).prev().val() < 20) {
		$(this).prev().val(+$(this).prev().val() + 1);
	}
});
$('.sub').click(function () {
	if ($(this).next().val() > 1) {
		if ($(this).next().val() > 1) $(this).next().val(+$(this).next().val() - 1);
	}
});



    
  

		})(window.jQuery);
		
