(function () {
    'use strict';

    // English text lives in the HTML; this holds the Arabic translations.
    var AR = {
        'nav.how': 'كيف يعمل', 'nav.features': 'المميزات', 'nav.drivers': 'انضم كسائق', 'nav.app': 'حمّل التطبيق', 'nav.login': 'تسجيل الدخول',
        'hero.eyebrow': 'تطبيق توصيل مصري',
        'hero.h1a': 'طريق واحد.', 'hero.h1b': 'رحلة واحدة.',
        'hero.lead': 'احجز مقعدك على طريقك، شارك الرحلة وشارك الأجرة. في سكتي هو البديل البسيط والعادل لأوبر وكريم.',
        'hero.cta1': 'حمّل التطبيق', 'hero.cta2': 'شوف إزاي بيشتغل',
        'trust.1': 'مسارات ثابتة', 'trust.2': 'أجرة مشتركة', 'trust.3': 'تتبّع مباشر',
        'float.1': 'سائقون موثّقون', 'float.2': 'شارك ووفّر',
        'how.title': 'كيف يعمل في سكتي', 'how.sub': 'ثلاث خطوات من باب بيتك لوجهتك.',
        'how.1t': 'اختر طريقك', 'how.1d': 'حدّد وجهتك وشوف الرحلات المتجهة ناحيتك.',
        'how.2t': 'احجز مقعدك', 'how.2d': 'احجز مقعدًا واحدًا، شوف الأجرة قبل الحجز وعرف ميعاد الاستلام.',
        'how.3t': 'اركب ووصّل', 'how.3d': 'تابع سائقك مباشرة، شارك رحلتك مع أهلك ووصّل في ميعادك.',
        'feat.title': 'مصمم لرحلاتك اليومية', 'feat.sub': 'كل ما تحتاجه لمشوار أفضل.',
        'feat.1t': 'مسارات ثابتة', 'feat.1d': 'مسارات ونقاط استلام واضحة، فتعرف دايمًا هتكون فين.',
        'feat.2t': 'شارك الأجرة', 'feat.2d': 'الركوب مع غيرك بيقسّم التكلفة، فكل مشوار أخف على جيبك.',
        'feat.3t': 'الأمان أولًا', 'feat.3d': 'سائقون موثّقون، مشاركة الرحلة ودعم داخل التطبيق في كل رحلة.',
        'feat.4t': 'تتبّع مباشر', 'feat.4d': 'شوف عربيتك وهي جاية لحظة بلحظة وتوصلك إشعارات عند الوصول.',
        'feat.5t': 'أسعار واضحة', 'feat.5d': 'تعرف الأجرة قبل ما تحجز. مفيش مفاجآت في الآخر.',
        'feat.6t': 'دعم محلي', 'feat.6d': 'فريق بيتكلم لغتك وجاهز يساعدك وقت ما تحتاج.',
        'drv.eyebrow': 'للسائقين', 'drv.title': 'اشتغل مع في سكتي',
        'drv.lead': 'املا مقاعدك الفاضية على الطرق اللي بتمشي فيها أصلًا. حدّد مواعيدك، واكسب أكتر مع كل رحلة مشتركة.',
        'drv.1': 'اختر طرقك ومواعيدك', 'drv.2': 'ركّاب أكتر في كل رحلة', 'drv.3': 'عرض بسيط للأرباح داخل التطبيق', 'drv.cta': 'كن سائقًا',
        'app.title': 'خُد في سكتي معاك', 'app.sub': 'تطبيق في سكتي لأندرويد وآيفون قريبًا.', 'app.soon': 'قريبًا', 'app.soon2': 'قريبًا',
        'foot.admin': 'دخول الفريق', 'foot.rights': 'جميع الحقوق محفوظة.'
    };

    var EN = {};
    var nodes = document.querySelectorAll('[data-i]');
    nodes.forEach(function (n) { EN[n.getAttribute('data-i')] = n.textContent; });

    var html = document.documentElement;
    var langBtn = document.getElementById('lang');

    function apply(lang) {
        var ar = lang === 'ar';
        html.lang = ar ? 'ar' : 'en';
        html.dir = ar ? 'rtl' : 'ltr';
        nodes.forEach(function (n) {
            var k = n.getAttribute('data-i');
            n.textContent = ar && AR[k] ? AR[k] : EN[k];
        });
        langBtn.textContent = ar ? 'English' : 'العربية';
        document.title = ar ? 'في سكتي – طريق واحد. رحلة واحدة.' : 'FeSekty – One route. One ride.';
        try { localStorage.setItem('fs-lang', lang); } catch (e) { /* storage may be blocked */ }
    }

    langBtn.addEventListener('click', function () { apply(html.dir === 'rtl' ? 'en' : 'ar'); });

    var saved = null;
    try { saved = localStorage.getItem('fs-lang'); } catch (e) { /* ignore */ }
    if (saved === 'ar') { apply('ar'); }

    // Mobile menu
    var burger = document.getElementById('burger');
    var nav = document.getElementById('nav');
    burger.addEventListener('click', function () {
        var open = nav.classList.toggle('open');
        burger.setAttribute('aria-expanded', open ? 'true' : 'false');
    });
    nav.addEventListener('click', function (e) {
        if (e.target.tagName === 'A') { nav.classList.remove('open'); burger.setAttribute('aria-expanded', 'false'); }
    });
})();
