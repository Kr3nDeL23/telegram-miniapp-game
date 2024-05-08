import TokenService from '@/services/tokenService';
import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
    history: createWebHistory(),
    routes: [
        {
            path: '/',
            meta: {
                requiresAuth: true
            },
            children: [
                {
                    path: '',
                    name: "index",
                    component: () => import('@/views/Index.vue')
                },
                {
                    path: 'boosts',
                    name: "boosts",
                    component: () => import('@/views/Boosts.vue')
                },
                {
                    path: 'profile',
                    name: "profile",
                    component: () => import('@/views/Profile.vue')
                },
                {
                    path: 'swap',
                    name: "swap",
                    component: () => import('@/views/Swap.vue')
                },
                {
                    path: 'leaderboard',
                    name: "leaderboard",
                    component: () => import('@/views/Leaderboard.vue')
                },
                {
                    path: 'earn',
                    name: "earn",
                    component: () => import('@/views/Earn.vue')
                },
            ]

        },
        {
            path: '/squad',
            meta: {
                requiresAuth: true
            },
            children: [
                {
                    name: "squad_detail",
                    path: 'detail/:id',
                    component: () => import('@/views/Squad/SquadDetail.vue')
                },
                {
                    name: "squad_list",
                    path: 'search',
                    component: () => import('@/views/Squad/SquadList.vue')
                },
            ]

        },
        {
            path: '/challenge',
            meta: {
                requiresAuth: true
            },
            children: [
                {
                    name: "challenge_detail",
                    path: 'detail/:id',
                    component: () => import('@/views/Challenge/ChallengeDetail.vue')
                },
            ]

        },
        {
            path: '/exception',

            children: [
                {
                    name: "exception",
                    path: ':status',
                    component: () => import('@/views/Exception.vue')
                },
            ]
        }
    ]
})
router.beforeEach((to, from, next) => {
    const tokenService = new TokenService();
    const platform = window.Telegram.WebApp.platform;
    const loaderElement = document.getElementById('loader');

    if (loaderElement) {
        loaderElement.style.display = 'flex';
    }

    const { path, name, meta } = to;
    const isAuthRequired = meta.requiresAuth;

    if (path === from.path && name === "index") {
        tokenService.clearStorage();
    }

    if (isAuthRequired && !tokenService.isAuthenticated()) {
        if (!["android", "ios"].includes(platform.toLowerCase())) {
            return next({ name: 'exception', params: { status: 401 } });
        }
        return tokenService.signIn(window.Telegram.WebApp.initData)
            .then(token => next())
            .catch(except => next({ name: 'exception', params: { status: 401 } }));
    }
    return next()


});

router.afterEach(() => {
    const loaderElement = document.getElementById('loader');
    if (loaderElement) {
        setTimeout(() => {
            loaderElement.style.display = 'none';
        }, 1000);
    }
    window.Telegram.WebApp.expand();


});

export default router
