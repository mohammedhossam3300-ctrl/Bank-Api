/* ============================================================
   site.js — Central script for Bank Management API frontend
   ============================================================ */

document.addEventListener('DOMContentLoaded', () => {

    /* ── Mobile hamburger nav ── */
    const hamburger = document.getElementById('nav-hamburger');
    const navLinks  = document.getElementById('nav-links');

    if (hamburger && navLinks) {
        hamburger.addEventListener('click', () => {
            const open = navLinks.classList.toggle('nav-open');
            hamburger.setAttribute('aria-expanded', open);
            hamburger.classList.toggle('open', open);
        });
        navLinks.querySelectorAll('a').forEach(link => {
            link.addEventListener('click', () => {
                navLinks.classList.remove('nav-open');
                hamburger.classList.remove('open');
                hamburger.setAttribute('aria-expanded', 'false');
            });
        });
    }

    /* ── Docs: tab navigation + scroll spy ── */
    const tabLinks = document.querySelectorAll('.tab-link');
    const sections = document.querySelectorAll('.resources');

    if (tabLinks.length && sections.length) {
        function updateActiveTab() {
            let current = '';
            sections.forEach(sec => {
                if (globalThis.scrollY >= sec.offsetTop - 230) {
                    current = sec.id;
                }
            });
            tabLinks.forEach(link => {
                link.classList.toggle('active', link.dataset.section === current);
            });
        }

        tabLinks.forEach(link => {
            link.addEventListener('click', e => {
                e.preventDefault();
                tabLinks.forEach(l => l.classList.remove('active'));
                link.classList.add('active');
                const target = document.getElementById(link.dataset.section);
                if (target) {
                    globalThis.scrollTo({ top: target.offsetTop - 190, behavior: 'smooth' });
                }
            });
        });

        globalThis.addEventListener('scroll', updateActiveTab, { passive: true });
        updateActiveTab();
    }

    /* ── Scroll-reveal via IntersectionObserver ── */
    if ('IntersectionObserver' in globalThis) {
        const observer = new IntersectionObserver(entries => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('revealed');
                    observer.unobserve(entry.target);
                }
            });
        }, { threshold: 0.08, rootMargin: '0px 0px -40px 0px' });

        document.querySelectorAll('[data-reveal]').forEach(el => observer.observe(el));
    } else {
        document.querySelectorAll('[data-reveal]').forEach(el => el.classList.add('revealed'));
    }
});
