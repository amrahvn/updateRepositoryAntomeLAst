
$(document).ready(function () {


    $(".select-option").click(function(){
        var ulElement = $(".select-option ul");
        ulElement.toggleClass("d-none");
    });

    $(".product-button li a").click(function(e) {
        $(".product-button li a").removeClass("active"); 
        $(this).addClass("active");
        e.preventDefault();
    });

    $('.product_tab_btn ul li a').click(function(e) {
        e.preventDefault(); 
        $('.product_tab_btn ul li a').removeClass('active');
        $(this).addClass('active');
    });
     
    $(".product-button ul li").click(function() {
      var targetId = $(this).data("target");
      $(".product-common").addClass("d-none");
      $(".product-common[data-id='" + targetId + "']").removeClass("d-none");
    });

    $('.menu--wrapper').slick({
        dots: false,
        infinite: true,
        arrows: true,
        // prevArrow:' <span><i class="fa-solid fa-chevron-left"></i></span>',
        // nextArrow: '<span><i class="fa-solid fa-chevron-right"></i></span>',
        speed: 800,
        slidesToShow: 5,
        slidesToScroll: 1,
        responsive: [
            {
                breakpoint: 1600,
                settings: {
                    slidesToShow: 5,
                }
            },
            {
                breakpoint: 1200,
                settings: {
                    slidesToShow: 6,
                }
            },
            {
                breakpoint: 992,
                settings: {
                    slidesToShow: 4,
                }
            },
            {
                breakpoint: 768,
                settings: {
                    slidesToShow: 3,
                }
            },
            {
                breakpoint: 576,
                settings: {
                    slidesToShow: 1,
                    arrows: false,
                }
            }
        ]
    });

    var miniCart = $('.mini_cart');
    var desiredHeight = 45 * parseFloat($('body').css('font-size')); 

    if (miniCart.height() > desiredHeight) {
        miniCart.css({
            'height': desiredHeight + 'px',
            'overflow-y': 'scroll'
        });
    }
});

document.addEventListener('DOMContentLoaded', function() {
    var activeIndex = 0; 
    var textChildElements = document.querySelectorAll('.text-child1'); 
    var carouselItems = document.querySelectorAll('.carousel-item'); 
    var intervalId; 

    function activateElement(index) {
        var activeElement = document.querySelector('.text-child1.active');
        activeElement.classList.remove('active');
    
        carouselItems.forEach(function(item) {
            item.classList.remove('active');
        });
       
        textChildElements[index].classList.add('active');
        carouselItems[index].classList.add('active');
    }

    textChildElements.forEach(function(element, index) {
        element.addEventListener('click', function (e) {
            e.preventDefault();

            clearInterval(intervalId);
            activeIndex = index; 
            activateElement(activeIndex); 
        });
        element.addEventListener('mouseover', function() {
            clearInterval(intervalId); 
        });
        element.addEventListener('mouseout', function() {
          
            intervalId = setInterval(function() {
                activeIndex = (activeIndex + 1) % textChildElements.length; 
                activateElement(activeIndex); 
            }, 4000); 
        });
    });

  
    intervalId = setInterval(function() {
        activeIndex = (activeIndex + 1) % textChildElements.length; 
        activateElement(activeIndex);
    }, 4000);
});


const miniCart = document.querySelector('.mini_cart');
const openBtn = document.getElementById('open');
const closeBtn = document.getElementById('closeBtn');

openBtn.addEventListener('click', function(e) {
    e.preventDefault();
    
    miniCart.style.transition = 'right 0.4s';
    miniCart.style.right = '0px';
});

closeBtn.addEventListener('click', function(e) {
    e.preventDefault();
   
    miniCart.style.transition = 'right 0.4s';
    miniCart.style.right = '-351px';
});


document.addEventListener('DOMContentLoaded', function() {
    const categoriesTitle = document.querySelector('.categories-title');
    const listGroup = document.querySelector('.list-group');

 
    if (listGroup.style.height !== '0px' && listGroup.style.height !== '') {
        listGroup.style.border = '2px solid #c40316';
    }

    categoriesTitle.addEventListener('click', function() {
        if (listGroup.style.height === '0px' || listGroup.style.height === '') {
            listGroup.style.height = '531px';
            listGroup.style.border = '2px solid #c40316';
        } else {
            listGroup.style.height = '0px';
        }

        listGroup.addEventListener('transitionend', function() {
            if (listGroup.style.height === '0px' || listGroup.style.height === '') {
                listGroup.style.border = '0px'; 
            } 
        });
    });
});

var fixedElement = document.querySelector('.containerrs');
var ulElement = document.querySelector('.header-menu-nav ul');

var scrollThreshold = 140;

window.addEventListener('scroll', function() {
    var scrollY = window.scrollY || window.pageYOffset;

    if (scrollY > scrollThreshold) {
        fixedElement.style.transition = '0.5s';
        fixedElement.style.position = 'fixed';
        fixedElement.style.top = '0';
        ulElement.style.marginLeft = '28px';
        ulElement.style.marginRight = '7px';
    } else {
        fixedElement.style.transition = '0.5s';
        fixedElement.style.position = 'static';
        ulElement.style.marginLeft = '0';
        ulElement.style.marginRight = '0';
    }
});




