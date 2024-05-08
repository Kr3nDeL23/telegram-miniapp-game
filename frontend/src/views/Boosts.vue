<template>
    <div class="mx-auto page_information flex-center flex-column text-center">
        <h1 class="text-color fs-large fw-bold">Boost</h1>
        <p class="description-color fs-small fw-bold mt-3">
            It increases the ability of the user account to extract easily and simply
        </p>
    </div>
    <div class="section_levels mt-5">
        <span class="text-color fs-medium fw-bold mb-3">Boosts</span>
        <div class="w-full d-flex flex-column">
            <BoostCard @upgrade="upgradedLevel" :type="LevelTypeEnum.LimitEnergyLevel" :user="user" :levels="levels" />
            <BoostCard @upgrade="upgradedLevel" :type="LevelTypeEnum.RechargeEnergyLevel" :user="user"
                :levels="levels" />
            <BoostCard @upgrade="upgradedLevel" :type="LevelTypeEnum.MultipleClickLevel" :user="user"
                :levels="levels" />

            <BoostCard @upgrade="upgradedLevel" :type="LevelTypeEnum.RoBotLevel" :user="user" :levels="levels" />
        </div>
    </div>
    <div class="section_skins mt-5">
        <span class="text-color fs-medium fw-bold mb-3">Skins</span>

        <div class="w-full d-flex flex-column">
            <SkinCard v-if="user && skins" v-for="(skin, index) in skins" :key="index" :skin="skin" :user="user"
                @click="selectSkin(skin)" />
            <SkinCard v-else v-for="(skin, i) in [null, null, null]" :key="i" :skin="skin" :user="user" />
        </div>
    </div>
    <div v-if="selected" class="section_confirm_buy w-full p-3">
        <div class="rounded-1 box-shadow confirm_buy_card p-3">
            <div class="d-flex flex-column">
                <span class="text-color fs-small fw-bold">Buy Skin</span>
                <span class="text-color fs-small fw-normal mt-1">Are you sure ?</span>
                <div class="flex-center mt-3">
                    <div @click="cancellButton" class="cancel_button box-shadow p-2 mr-1 rounded-1 flex-center">
                        <span class="text-color fs-small fw-bold ">Cancel</span>
                    </div>
                    <div @click="confirmButton" class="confirm_button box-shadow p-2 mr-1 rounded-1 flex-center">
                        <span class="text-color fs-small fw-bold ">Confirm</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script setup lang="ts">
import { ref, inject } from "vue";

import { LevelTypeEnum } from '@/models/levelModel';
import SkinCard from '@/components/cards/SkinCard.vue';
import BoostCard from '@/components/cards/BoostCard.vue';

import type UserModel from "@/models/userModel";
import type LevelModel from "@/models/levelModel";

import UserService from "@/services/userService";
import LevelService from "@/services/levelService";
import type SkinModel from "@/models/skinModel";
import SkinService from "@/services/skinService";
import { NotificationTypeEnum } from "@/models/notificationModel";

const user = ref<UserModel | null>(null);
const selected = ref<SkinModel | null>(null);
const skins = ref<Array<SkinModel> | null>(null);
const levels = ref<Array<LevelModel> | null>(null);

const userService = new UserService();
const skinService = new SkinService();
const levelService = new LevelService();

const { createNotification } = inject("notification");

const fetchRequest = () => {
    levelService.getLevels().then(result => {
        levels.value = result;
    });
    userService.profile().then(result => {
        user.value = result;
    });

    skinService.getList().then(result => {
        skins.value = result;
    });
};
const selectSkin = (skin: SkinModel) => {
    if (!user.value || !skin) return;
    if (user.value.skinId == skin.id) return;

    if (skin.isBought) {
        createNotification({
            title: "please wait",
            description: "The request is in progress ...",
            type: NotificationTypeEnum.Waiting,
        });
        skinService.set(skin.id)
            .then(result => {
                user.value = result;
                createNotification({
                    title: "Skin Changed",
                    description: "TSkin changed successfully",
                    type: NotificationTypeEnum.Success,
                });
            }).catch(except => createNotification({
                title: "Not Changed",
                description: "Can not change skin",
                type: NotificationTypeEnum.Exception,
            }));
    } else {
        selected.value = skin;
    }
};
const confirmButton = () => {
    if (!selected.value || !user.value) return;
    if (selected.value.isBought) return;

    const id = selected.value.id;

    if (selected.value.availableCoin > user.value.balanceCoin) {
        selected.value = null;

        createNotification({
            title: "Balance Error",
            description: "Balance is not enough",
            type: NotificationTypeEnum.Exception,
        });
        return;
    }
    selected.value = null;


    createNotification({
        title: "please wait",
        description: "The request is in progress ...",
        type: NotificationTypeEnum.Waiting,
    });
    skinService.buy(id)
        .then(result => {
            createNotification({
                title: "Successful purchase",
                description: "The purchase was made successfully",
                type: NotificationTypeEnum.Success,
            });
            user.value = result;
            if (skins.value) {
                const findSkin = skins.value.find(x => x.id == id);
                if (findSkin) findSkin.isBought = true;
            }
        })
        .catch(except => {
            if (except.responseStatus = 400) {
                createNotification({
                    title: "Balance Error",
                    description: "Balance is not enough",
                    type: NotificationTypeEnum.Exception,
                });
                return;
            }
            createNotification({
                title: "Skin Error",
                description: "Can not buy skin",
                type: NotificationTypeEnum.Exception,
            });
        })
};
const cancellButton = () => {
    selected.value = null;
};
const upgradedLevel = () => {
    userService.profile().then(result => user.value = result)
};
fetchRequest()
</script>

<style scoped>
@keyframes ShowNotification {
    0% {
        bottom: -200px;
    }

    100% {
        bottom: 0;
    }
}

.boost_icon {
    width: 50px;
    height: 50px;
}

.section_confirm_buy {
    left: 0;
    bottom: 0;
    position: fixed;
    animation: ShowNotification alternate var(--transition-fast);

}

.confirm_buy_card {
    background: hsl(var(--theme-color));
}

.cancel_button {
    background: rgb(var(--color-error));
}

.confirm_button,
.cancel_button {
    flex: 1 0 0%;
}

.confirm_button {
    background: rgb(var(--color-success));
}
</style>