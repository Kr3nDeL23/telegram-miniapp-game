<template>
    <div class="mx-auto page_information flex-center flex-column text-center">
        <h1 class="text-color fs-large fw-bold">Leagues</h1>
        <p class="description-color fs-small fw-bold mt-3">
            List of available leagues with users and competing teams
        </p>
    </div>
    <div class="section_leagues mt-4">
        <div class="box-shadow bg-card p-3 rounded-1">
            <span class="text-color fs-medium fw-bold">Leagues</span>
            <div class="d-flex flex-column mt-3">
                <div v-if="leagues" v-for="(league, index) in leagues" :key="index"
                    class="d-flex align-items-center mb-3" :class="isSelected(league.id) ? `selected` : ``"
                    @click="selectLeague(league.id)">
                    <div class="league_image flex-center mr-2 bg-card box-shadow p-2 rounded-1">
                        <img class="w-full h-full" :src="$filters.serverLinkFormat(league.image)" alt="">
                    </div>
                    <div class="d-flex flex-column">
                        <span class="text-color fs-small fw-bold">{{ league.name }} League</span>
                        <span class="mt-2 description-color fs-small fw-normal">
                            from <b class="theme-color">{{ league.availableCoin }}</b> balance coin
                        </span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <section class="mt-3">
        <div class="box-shadow bg-card p-3 rounded-1">
            <div class="flex-center mb-3">
                <div class="flex-center bg-card px-2 py-3 mr-1 select_fetch_user box-shadow rounded-1"
                    :class="userSelect ? `selected` : ``" @click="getUsers">
                    <span class="text-color fs-small fw-bold">Users</span>
                </div>
                <div class="flex-center bg-card px-2 py-3 ml-1 select_fetch_squads box-shadow rounded-1"
                    :class="!userSelect ? `selected` : ``" @click="getSquads">
                    <span class="text-color fs-small fw-bold">Squads</span>
                </div>

            </div>
            <SearchBox @change="changeQueryData" :title="userSelect ? 'Users' : 'Squads'" />

            <UserCardList v-if="userSelect" class="mt-3" :users="members" />
            <SquadCardList v-else class="mt-3" :squads="squads" />
        </div>
    </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';

import LeagueService from '@/services/leagueService';

import type UserModel from '@/models/userModel';
import type LeagueModel from '@/models/leagueModel';

import SearchBox from '@/components/search/SearchBox.vue';
import UserCardList from '@/components/cards/UserCardList.vue';
import SquadCardList from '@/components/cards/SquadCardList.vue';
import type SquadModel from '@/models/squadModel';

const leagueService = new LeagueService();

const query = ref<string>("");
const userSelect = ref<boolean>(true);
const selected = ref<LeagueModel>();
const squads = ref<Array<SquadModel> | null>(null);
const members = ref<Array<UserModel> | null>(null);
const leagues = ref<Array<LeagueModel> | null>(null);

const isSelected = (id: string) => {
    if (selected.value && selected.value.id == id) return true;

    return false
};
const selectLeague = (id: string) => {
    if (!leagues.value) return;

    selected.value = leagues.value.find(x => x.id == id);
    if (userSelect.value) getUsers();
    else getSquads();
};

const fetchLeagues = () => {
    leagueService.getList().then(result => {
        leagues.value = result;
        if (result && result.length > 0) {
            selected.value = result[0];
            fetchUsers(selected.value.id);
        }
    });
};

const fetchUsers = (id: string) => {

    leagueService.getMembers(id, query.value).then(result => {
        members.value = result;
    });
};

const fetchSquads = (id: string) => {
    leagueService.getSquads(id, query.value).then(result => {
        squads.value = result;
    });
};
const getUsers = () => {
    userSelect.value = true;
    if (!selected.value) return;

    fetchUsers(selected.value.id);
};

const getSquads = () => {
    userSelect.value = false;
    if (!selected.value) return;

    fetchSquads(selected.value.id);
};
const changeQueryData = (q: string) => {
    query.value = q;
    const currentValue = query.value;

    if (q.length == 0) {
        squads.value = new Array<SquadModel>();
        members.value = new Array<UserModel>();
        return;
    }
    if (q.length < 3) return;

    squads.value = null;
    members.value = null;

    setTimeout(() => {
        if (query.value == currentValue) {
            if (userSelect.value) getUsers();
            else getSquads();
        }
    }, 3000);

};

fetchLeagues();
</script>

<style scoped>
.league_image {
    min-width: 55px;
    min-height: 55px;
    max-width: 55px;
    max-height: 55px;
}

.selected {
    opacity: 0.5;
}

.select_fetch_user,
.select_fetch_squads {
    flex: 1 0 0%;
}
</style>