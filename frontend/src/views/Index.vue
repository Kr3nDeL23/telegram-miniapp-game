<template>
    <section class="section_squad">
        <div class="box-shadow bg-card p-3 rounded-1">
            <div v-if="squad" @click="$router.push({ name: 'squad_detail', params: { id: squad.id } })"
                class="d-flex align-items-center justify-content-between">
                <div class="flex-center">
                    <div class="squad_profile d-flex mr-3">
                        <img class="rounded-1 w-full h-full" :src="$filters.serverLinkFormat(squad.image)" alt="">
                    </div>
                    <div class="d-flex flex-column">
                        <span class="text-color fs-small fw-bold">{{ $filters.truncate(squad.name, 30) }}</span>
                        <span class="description-color fs-small fw-normal mt-2">
                            <div class="d-flex align-items-center">
                                <div class="d-flex align-items-center mr-3">
                                    <img class="mr-1" width="15px" src="@/assets/images/icons/coin.svg" alt="">
                                    {{ $filters.numberFormat(squad.balanceCoin) }}
                                </div>
                                <div class="d-flex align-items-center">
                                    <img class="mr-1" width="15px" :src="$filters.serverLinkFormat(squad.league.image)"
                                        alt="">
                                    {{ squad.league.name }}
                                </div>
                            </div>

                        </span>
                    </div>
                </div>
            </div>
            <div v-else @click="$router.push({ name: 'squad_list' })" class="flex-center">
                <span class="text-color fs-medium fw-normal">Join Squad</span>
            </div>
        </div>
    </section>
    <div class="section_balance mt-5">
        <div class="flex-center">
            <img class="mr-2" width="35px" src="@/assets/images/icons/coin.svg" alt="">
            <span class="text-color fs-secondary-large fw-bold">{{ $filters.numberFormat(user?.balanceCoin ?? 0)
                }}</span>
        </div>
        <div class="flex-center mt-2" @click="$router.push({ name: 'leaderboard' })">
            <img class="mr-1" width="20px" :src="$filters.serverLinkFormat(user?.league.image)" alt="">
            <span class="description-color fs-small fw-bold">{{ user?.league.name }}</span>
        </div>
    </div>
    <section class="section_click mt-5">
        <div class="flex-center">
            <div class="section_skin_image" @click="clickRequest">
                <img class="w-full h-full" :src="$filters.serverLinkFormat(user?.skin.image)" alt="">
                <span v-for="(score, index) in scores" :key="index" :style="`top: ${score.y}px; left:${score.x}px;`"
                    class="text-color fs-secondary-large fw-bold recived_score">{{ score.score }}</span>
            </div>
        </div>
    </section>
    <div class="section_footer mb-2 mt-auto">

        <div class="d-flex flex-column w-full">
            <div class="d-flex align-items-center mb-2">
                <img width="30px" src="@/assets/images/icons/clock.png" alt="">
                <span class="text-color fs-medium fw-normal ml-2"><b>{{ $filters.numberFormat(user?.availableEnergy ??
                    0) }}</b> /
                    {{ $filters.numberFormat(user?.limitEnergyLevel.value ?? 0) }}</span>
            </div>

            <ProgressLine class="progress_energy" :value="user?.availableEnergy ?? 100"
                :size="user?.limitEnergyLevel.value ?? 100" />
        </div>


        <div class="mt-4 box-shadow bg-card rounded-1 py-2">
            <div class="d-flex align-items-center justify-content-between">
                <div @click="$router.push({ name: 'earn' })" class="footer_item flex-center flex-column px-3">
                    <img width="35px" src="@/assets/images/icons/earn.png" alt="leaderboard">
                    <span class="text-color fs-secondary-small fw-bold mt-2">Earn</span>
                </div>
                <div class="footer_spliter"></div>

                <div @click="$router.push({ name: 'boosts' })" class="footer_item flex-center flex-column px-3">
                    <img width="35px" src="@/assets/images/icons/boost.png" alt="leaderboard">
                    <span class="text-color fs-secondary-small fw-bold mt-2">Boosts</span>
                </div>
                <div class="footer_spliter"></div>
                <div @click="$router.push({ name: 'leaderboard' })" class="footer_item flex-center flex-column px-3">
                    <img width="35px" src="@/assets/images/icons/leaderboard.png" alt="leaderboard">
                    <span class="text-color fs-secondary-small fw-bold mt-2">Leaders</span>
                </div>
                <div class="footer_spliter"></div>
                <div @click="$router.push({ name: 'profile' })" class="footer_item flex-center flex-column px-3">
                    <img width="35px" src="@/assets/images/icons/profile.png" alt="leaderboard">
                    <span class="text-color fs-secondary-small fw-bold mt-2">Profile</span>
                </div>
            </div>
        </div>

    </div>
</template>

<script lang="ts" setup>
import type UserModel from '@/models/userModel';
import type SquadModel from '@/models/squadModel';

import UserService from '@/services/userService';
import GameService from '@/services/gameService';

import ProgressLine from '@/components/utilities/ProgressLine.vue';

import { ref, onMounted, inject } from 'vue'
import { NotificationTypeEnum } from '@/models/notificationModel';

const { createNotification } = inject("notification");

const user = ref<UserModel>();
const squad = ref<SquadModel>();

const miningCount = ref<number>(0);
const lastClickDate = ref<Date | null>();

const scores = ref<Array<object>>([]);

const gameService = new GameService();
const userService = new UserService();


onMounted(() => {
    const id = setInterval(() => {
        if (user.value && user.value.availableEnergy != user.value.limitEnergyLevel.value) {
            user.value.availableEnergy += user.value.rechargeEnergyLevel.value;

            if (user.value.availableEnergy > user.value.limitEnergyLevel.value) {
                user.value.availableEnergy = user.value.limitEnergyLevel.value;
            }

        }

        if (lastClickDate.value && new Date().getTime() - lastClickDate.value.getTime() >= 2000) {

            if (miningCount.value != 0)
                new GameService().sendClick(miningCount.value)
                    .finally(() => getProfileRequest());

            miningCount.value = 0;
            lastClickDate.value = null;

        }
    }, 1000);
    return () => clearInterval(id);

});

const getProfileRequest = () => {

    userService.profile()
        .then(result => {
            user.value = result;
            squad.value = result.squad;
        })
};
const getCheckRobotRequest = () => {
    setTimeout(() => {
        if (user.value && user.value.roBotLevel) {
            gameService.checkRoBot()
                .then(result => {
                    if (user.value) {
                        user.value.balanceCoin += result;
                        if (user.value.squad) {
                            user.value.squad.balanceCoin += result;
                        }
                    }
                    createNotification({
                        title: "Robot Mining",
                        description: `In your absence, the robot has collected ${result} coins for you`,
                        type: NotificationTypeEnum.Information,
                    });
                });
        }

    }, 3000);

};

const clickRequest = (e: any) => {
    if (!user.value) return;

    if (user.value.availableEnergy < user.value.multipleClickLevel.value)
        return;

    const x = e.offsetX;
    const y = e.offsetY;

    const score = { x, y, score: getCountCoinForAnyClick() };

    scores.value.push(score);

    setTimeout(() => {
        let index = scores.value.indexOf(score);
        if (index > -1)
            scores.value.splice(index, 1);
    }, 1000);

    const countCoin = getCountCoinForAnyClick();

    user.value.balanceCoin += countCoin;
    user.value.availableEnergy -= countCoin;

    if (squad.value) {
        squad.value.balanceCoin += countCoin;
    }
    miningCount.value += countCoin;

    lastClickDate.value = new Date();

};

const getCountCoinForAnyClick = () => {
    return user.value?.multipleClickLevel.value ?? 1;
};

getProfileRequest();
getCheckRobotRequest();


</script>

<style scoped>
.squad_profile {
    min-width: 55px;
    min-height: 55px;
    max-width: 55px;
    max-height: 55px;
}

.squad_profile img {
    width: 100%;
    height: 100%;
}

.section_skin_image {
    width: 100%;
    max-width: 280px;
    position: relative;
}

.section_skin_image img {
    transition: ease all var(--transition-fast);
}

.footer_spliter {
    width: 1px;
    height: 50px;
    background: rgba(var(--text-color), 0.4);
}

.progress_energy {
    height: 20px;

}

.footer_item {
    flex: 1 0 0%;
}

@keyframes AnimationShowScore {
    0% {
        opacity: 0;
    }

    50% {
        opacity: 1;
    }

    100% {
        opacity: 0;
        transform: translateY(-100px);
    }
}


.recived_score {
    position: absolute;
    pointer-events: none;
    z-index: var(--z-index-fixed);
    animation: AnimationShowScore both var(--transition-slow);
}

.section_skin_image:active img {
    transform: scale(0.95);
}
</style>