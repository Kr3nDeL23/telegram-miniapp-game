<template>
    <section class="d-flex flex-column">
        <div class="mx-auto page_information flex-center flex-column text-center">
            <h1 class="text-color fs-large fw-bold">Join Squad</h1>
            <p class="description-color fs-small fw-bold mt-3">These groups are actively recruiting hackers, take
                your
                pick</p>
        </div>
        <div class="bg-card box-shadow rounded-1 mt-5 p-3">
            <div class="w-full d-flex flex-column">
                <span class="fs-medium fw-bold text-color">Create Squad</span>
                <span class="description-color fs-small fw-normal">create or join to other squad</span>
                <button @click="joinOtherSquad()"
                    class="create_squad_button text-color mt-3 fs-medium p-2 fw-bold rounded-1">
                    Create Squad
                </button>
            </div>
        </div>

    </section>
    <section class="section_squads mt-4">
        <div class="box-shadow bg-card p-3 rounded-1">
            <SearchBox @change="changeQueryData" title="Top Squads" />
            <SquadCardList class="mt-3" :squads="squads" />
        </div>
    </section>
</template>
<script setup lang="ts">
import { ref } from 'vue';

import type SquadModel from '@/models/squadModel';

import SquadService from '@/services/squadService';

import SearchBox from '@/components/search/SearchBox.vue';
import SquadCardList from '@/components/cards/SquadCardList.vue';
import { BOT_URL } from '@/configurations/HttpConfiguration';

const query = ref<string>("");
const squads = ref<Array<SquadModel> | null>(null);

const squadService = new SquadService();

const getSquadListRequest = () => {
    squadService.getList(query.value)
        .then(result => squads.value = result.result);
};

const changeQueryData = (q: string) => {
    query.value = q;
    const currentValue = query.value;

    if (q.length == 0) {
        squads.value = new Array<SquadModel>();
        return;
    }
    if (q.length < 3) return;

    squads.value = null;

    setTimeout(() => {
        if (query.value == currentValue) {
            getSquadListRequest();
        }
    }, 3000);
};
const joinOtherSquad = () => {
    window.Telegram.WebApp.openTelegramLink(BOT_URL + '?start=createsquad');
    window.Telegram.WebApp.close();
}
getSquadListRequest();
</script>

<style scoped>
.create_squad_button {
    background: hsl(var(--theme-color));
}
</style>