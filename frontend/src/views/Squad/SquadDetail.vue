<template>
    <section class="section_squad">
        <div class="box-shadow bg-card p-3 rounded-1">
            <div class="d-flex align-items-center justify-content-between">
                <div class="flex-center">
                    <div class="squad_profile d-flex mr-3">
                        <img v-if="squad" class="rounded-circle w-full h-full"
                            :src="$filters.serverLinkFormat(squad.image)" alt="">
                        <SkeletonLoader v-else width="100%" height="55px" />
                    </div>
                    <div class="d-flex flex-column">

                        <span v-if="squad" class="text-color fs-small fw-bold">{{ $filters.truncate(squad.name, 30)
                            }}</span>
                        <SkeletonLoader v-else width="200px" height="20px" />

                        <span class="description-color fs-small fw-normal mt-2">
                            <div v-if="squad" class="d-flex align-items-center">
                                <div class="d-flex align-items-center mr-3">
                                    <img class="mr-1" width="15px" src="@/assets/images/icons/coin.svg" alt="">
                                    {{ $filters.numberFormat(squad?.balanceCoin) }}
                                </div>
                                <div class="d-flex align-items-center">
                                    <img class="mr-1" width="15px" :src="$filters.serverLinkFormat(squad?.league.image)"
                                        alt="">
                                    {{ squad?.league.name }}
                                </div>
                            </div>
                            <SkeletonLoader v-else width="100px" height="20px" />
                        </span>

                    </div>
                </div>

                <a :href="squad ? `https://t.me/${squad.userName}` : ``" class="flex-center">
                    <img class="open_link_image" src="@/assets/images/icons/open_link.png" alt="">
                </a>
            </div>
            <div class="d-flex flex-column mt-4">
                <span class="fs-small text-color fw-bold mb-2">Invite Link</span>
                <LinkBox v-if="squad" class="w-full" :link="getInviteLink()" />
                <SkeletonLoader v-else width="100%" height="40px" />
            </div>
            <div v-if="joined" @click="leftSquadRequest"
                class="leave_group_button flex-center box-shadow p-3 rounded-1 mt-3">
                <span class="text-color fs-small fw-bold">Leave group</span>
            </div>
            <div v-else @click="joinSquadRequest" class="join_group_button flex-center box-shadow p-3 rounded-1 mt-3">
                <span class="text-color fs-small fw-bold">Join group</span>
            </div>
        </div>
    </section>
    <section class="section_members mt-3">
        <div class="box-shadow bg-card p-3 rounded-1">
            <SearchBox @change="changeQueryData" title="Members" />
            <UserCardList class="mt-3" :users="members" />
        </div>
    </section>
</template>

<script setup lang="ts">
import { ref } from 'vue';

import type UserModel from '@/models/userModel';
import type SquadModel from '@/models/squadModel';

import UserService from '@/services/userService';
import SquadService from '@/services/squadService';

import LinkBox from '@/components/utilities/LinkBox.vue';
import SearchBox from '@/components/search/SearchBox.vue';
import UserCardList from '@/components/cards/UserCardList.vue';
import SkeletonLoader from '@/components/utilities/SkeletonLoader.vue';

import router from '@/router';
import { useRoute } from 'vue-router';
import { BOT_URL } from '@/configurations/HttpConfiguration';

const route = useRoute();

const query = ref<string>("");
const squad = ref<SquadModel>();
const joined = ref<boolean>(false);
const members = ref<Array<UserModel> | null>(null);

const id = route.params.id.toString();

const squadService = new SquadService();

const getSquadRequest = () => {
    const userFromStorage = new UserService().getUserFromStorage();

    if (userFromStorage && userFromStorage.squadId && userFromStorage.squadId == id) {
        joined.value = true;
        squad.value = userFromStorage.squad;

        getMembers();
    } else squadService.getSquad(id)
        .then(result => {
            squad.value = result;
            getMembers()
        })
        .catch(except => {
            if (except.responseStatus == 404) {
                router.push({ name: 'exception', params: { status: 404 } });
            }
        });
};
const getMembers = (q: string = query.value) => {
    squadService.getMembers(id, q)
        .then(result => members.value = result.result);
};
const getInviteLink = () => {
    if (!squad.value) return '';

    return BOT_URL + "/?start=join_" + squad.value.id;
};

const joinSquadRequest = () => {
    squadService.Join(id)
        .then(result => {
            joined.value = true;
            squad.value = result.squad;

            members.value?.push(result);
        });
};
const leftSquadRequest = () => {
    squadService.Left()
        .then(result => {
            joined.value = false;

            const index = members.value?.findIndex(x => x.id == result.id) ?? -1;
            if (index > -1)
                members.value?.splice(index);
        });
};

const changeQueryData = (q: string) => {
    query.value = q;
    const currentValue = query.value;

    if (q.length == 0) {
        members.value = new Array<UserModel>();
        return;
    }
    if (q.length < 3) return;

    members.value = null;

    setTimeout(() => {
        if (query.value == currentValue) {
            members.value = null;
            getMembers();
        }
    }, 3000);
};
getSquadRequest()

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

.leave_group_button {
    background-color: rgb(var(--color-error));
}

.join_group_button {
    background-color: rgb(var(--color-success));
}

.open_link_image {
    min-width: 30px;
    max-width: 30px;
}
</style>