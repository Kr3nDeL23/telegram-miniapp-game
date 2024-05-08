<template>
    <div class="mx-auto page_information flex-center flex-column text-center">
        <h1 class="text-color fs-large fw-bold">Earn</h1>
        <p class="description-color fs-small fw-bold mt-3">
            Get free coins easily by doing a series of simple tasks
        </p>
    </div>

    <section class="section_invite mt-5">
        <div @click="$router.push({ name: 'profile' })" class="mt-3 bg-card box-shadow p-2 rounded-1">
            <div class="d-flex align-items-center justify-content-between">
                <div class="flex-center">
                    <div class="card_icon flex-center bg-card mr-2 rounded-1 p-2 box-shadow">
                        <img class="w-full h-full" src="@/assets/images/icons/invitation.png" alt="">
                    </div>
                    <div class="d-flex flex-column">
                        <span class="text-color fs-small fw-bold mb-2">Invite Friends Bonus</span>
                        <div class="d-flex align-items-center">
                            <span class="description-color fs-small fw-normal">
                                Invite friends to get <b class="theme-color">2500</b> coins
                            </span>

                        </div>
                    </div>
                </div>

            </div>

        </div>
    </section>
    <section class="section_challenges mt-5">
        <span class="fs-medium fw-bold text-color">Challenges</span>
        <div class="d-flex flex-column">
            <div v-for="(challenge, index) in challenges" :key="index" class="mt-3 bg-card box-shadow p-2 rounded-1">
                <div @click="challenge.isCompleted ? () => { } : $router.push({ name: 'challenge_detail', params: { id: challenge.id } })"
                    :class="challenge.isCompleted ? 'completed' : ''"
                    class="d-flex align-items-center justify-content-between">
                    <div class="flex-center">
                        <div class="card_icon flex-center bg-card mr-2 rounded-1 p-2 box-shadow">
                            <img class="w-full h-full" :src="$filters.serverLinkFormat(challenge.image)" alt="">
                        </div>
                        <div class="d-flex flex-column">
                            <span class="text-color fs-small fw-bold mb-2">{{ challenge.name }}</span>
                            <div class="d-flex align-items-center mr-3">
                                <img v-if="!challenge.isCompleted" class="mr-1" width="15px" src="@/assets/images/icons/coin.svg" alt="">
                                <span class="description-color fs-small fw-bold">
                                    {{ challenge.isCompleted ? 'completed' : $filters.numberFormat(challenge.bonus) }}
                                </span>

                            </div>
                        </div>
                    </div>

                </div>

            </div>
        </div>
    </section>
</template>


<script lang="ts" setup>
import { ref } from 'vue';
import type ChallengeModel from '@/models/challengeModel';
import ChallengeService from '@/services/challengesService';

const challenges = ref<Array<ChallengeModel>>(new Array<ChallengeModel>())

const challengeService = new ChallengeService();

challengeService.getList()
    .then(result => challenges.value = result);

</script>

<style scoped>
.completed {
    opacity: 0.5;
}
</style>