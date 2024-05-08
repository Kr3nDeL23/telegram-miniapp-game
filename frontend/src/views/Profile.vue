<template>
    <div class="section_balance">
        <div class="flex-center">
            <span class="description-color fs-small fw-normal">Your Coin Balance</span>
        </div>
        <div class="flex-center mt-4">
            <img class="mr-2" width="35px" src="@/assets/images/icons/coin.svg" alt="" />
            <span class="text-color fs-secondary-large fw-bold">{{
                $filters.numberFormat(user?.balanceCoin ?? 0)
            }}</span>
        </div>
    </div>
    <section class="secttion_profile mt-5">
        <div class="flex-center flex-column bg-card px-2 box-shadow rounded-1">
            <UserCard :user="user" />

            <div class="w-full d-flex flex-column mt-2 py-3">
                <span class="fs-small text-color fw-bold">Invite Link</span>
                <span class="fs-secondary-small description-color fw-bold mt-1  mb-2">Invite friends and get 2,500
                    coin</span>
                <LinkBox v-if="user" class="w-full" :link="getReferralLink()" />
                <SkeletonLoader v-else width="100%" height="40px" />
            </div>
            <div class="w-full d-flex flex-column mt-3">
                <span class="fs-small text-color fw-bold">Swap & Transfer Coin</span>
                <span class="fs-secondary-small description-color fw-bold mt-1  mb-2">
                    Transfer or convert your assets to other currencies
                </span>
                <div class="flex-center w-full pb-3 mt-1">
                    <div @click="$router.push({ name: 'swap' })"
                        class="user_profile_button flex-center bg-card px-2 py-2 mr-1 box-shadow rounded-1">
                        <span class="text-color fs-small fw-bold flex-center">
                            <SwapIcon class="mr-2" width="16px" height="16px" />
                            Swap
                        </span>
                    </div>
                    <div @click="$router.push({ name: 'swap' })"
                        class="user_profile_button flex-center bg-card px-2 py-2 ml-1 box-shadow rounded-1">
                        <span class="text-color fs-small fw-bold flex-center">
                            <TransferIcon class="mr-2" width="16px" height="16px" />
                            Transfer
                        </span>
                    </div>

                </div>
            </div>
        </div>
    </section>

    <section class="section_members mt-3">
        <div class="box-shadow bg-card p-3 rounded-1">
            <SearchBox @change="changeQueryData" title="Referrals" :description="members ? `count - ${$filters.numberFormat(members.length)}`
                : 'loading'" />

            <UserCardList class=" mt-3" :users="members" />
        </div>
    </section>
</template>

<script setup lang="ts">
import { inject, ref } from "vue";

import UserService from "@/services/userService";

import UserCard from "@/components/cards/UserCard.vue";
import LinkBox from "@/components/utilities/LinkBox.vue";
import SkeletonLoader from "@/components/utilities/SkeletonLoader.vue";

import type UserModel from "@/models/userModel";
import UserCardList from "@/components/cards/UserCardList.vue";
import SearchBox from "@/components/search/SearchBox.vue";
import { BOT_URL } from "@/configurations/HttpConfiguration";
import SwapIcon from "@/components/icons/SwapIcon.vue";
import TransferIcon from "@/components/icons/TransferIcon.vue";

const query = ref<string>("");

const countMembers = ref<number>(0);

const user = ref<UserModel | null>(null);
const members = ref<Array<UserModel> | null>(null);

const userService = new UserService();

const getProfileRequest = () => {
    userService.profile()
        .then((result) => (user.value = result));
    getMembers();
};

const getMembers = () => {
    userService.getMembers(query.value)
        .then((result) => {
            members.value = result.result
            countMembers.value = result.countTotal;
        });
};


const getReferralLink = () => {
    if (!user.value) return "";

    return BOT_URL + "/?start=ref_" + user.value.id;
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
getProfileRequest();
</script>

<style scoped>
.user_profile_button {
    flex: 1 0 0%;
}
</style>
