<template>
    <div class="mt-3 bg-card box-shadow p-2 rounded-1">

        <div @click="selecteLevel" :class="!getUpgradeLevel() ? 'max_level' : ''"
            class="d-flex align-items-center justify-content-between">
            <div class="flex-center">
                <div class="boost_icon flex-center bg-card mr-2 rounded-1 p-2 box-shadow">
                    <img v-if="props.type == LevelTypeEnum.LimitEnergyLevel" class="w-full h-full"
                        src="@/assets/images/icons/hourglass.png" alt="">
                    <img v-else-if="props.type == LevelTypeEnum.MultipleClickLevel" class="w-full h-full"
                        src="@/assets/images/icons/tap.png" alt="">
                    <img v-else-if="props.type == LevelTypeEnum.RechargeEnergyLevel" class="w-full h-full"
                        src="@/assets/images/icons/charging.png" alt="">

                    <img v-else class="w-full h-full" src="@/assets/images/icons/robot.png" alt="">
                </div>
                <div class="d-flex flex-column">
                    <span class="text-color fs-small fw-bold mb-2">{{ getLevelName() }}</span>
                    <div v-if="user && levels && getUpgradeLevel()" class="d-flex align-items-center">
                        <span class="description-color fs-small fw-normal">
                            Up to {{ $filters.numberFormat(getUpgradeLevel()?.value ?? 0) }}
                        </span>

                        <div class="flex-center ml-2">
                            <img width="15px" src="@/assets/images/icons/coin.svg" alt="">
                            <span class="text-color fs-small fw-normal ml-1">
                                <b class="text-color">+</b>
                                {{ $filters.numberFormat(getUpgradeLevel()?.availableCoin ?? 0) }}
                            </span>
                        </div>
                    </div>
                    <span v-else class="description-color fs-small fw-normal">Is Max</span>
                </div>
            </div>
            <div class="flex-center">
                <img width="35px" src="@/assets/images/icons/improve.png" alt="">
                <span v-if="user" class="text-color fs-small fw-normal">
                    Level {{ getCurrenctLevel() }}
                </span>
                <SkeletonLoader v-else width="30px" height="15px" />
            </div>
        </div>
        <div v-if="isSelected && getUpgradeLevel()" class="mt-4 upgrade_skin d-flex flex-column">
            <span class="text-color fs-small fw-bold">Upgrade Level</span>
            <span class="description-color fs-small fw-normal mt-1">Are You Sure to upgrade ?</span>

            <div class="flex-center mt-3">
                <div @click="cancelUpgrade" class="cancel_button box-shadow p-2 mr-1 rounded-1 flex-center">
                    <span class="text-color fs-small fw-bold">Cancel</span>
                </div>
                <div @click="confirmUpgrade" class="confirm_button box-shadow p-2 mr-1 rounded-1 flex-center">
                    <span class="text-color fs-small fw-bold ">Confirm</span>
                </div>
            </div>
        </div>
    </div>
</template>
<script setup lang="ts">
import { ref, inject } from 'vue';

import { LevelTypeEnum } from '@/models/levelModel';
import { NotificationTypeEnum } from '@/models/notificationModel';

import LevelService from '@/services/levelService';
import SkeletonLoader from '@/components/utilities/SkeletonLoader.vue';


import type UserModel from '@/models/userModel';
import type LevelModel from '@/models/levelModel';

const levelService = new LevelService();

const { createNotification } = inject("notification");

const emit = defineEmits<{
    (e: 'upgrade'): void
}>()

interface Props {
    type: LevelTypeEnum,
    user: UserModel | null,
    levels: Array<LevelModel> | null,
}
const props = defineProps<Props>()

const isSelected = ref<boolean>(false);

const getUpgradeLevel = () => {
    if (!props.user || !props.levels) return null;

    const currentLevel = getCurrenctLevel();
    return props.levels.find(x => x.levelType == props.type && x.level == currentLevel + 1);
};
const selecteLevel = () => {
    isSelected.value = true;
};
const cancelUpgrade = () => {
    isSelected.value = false;
};
const getLevelName = () => {
    return props.type == LevelTypeEnum.LimitEnergyLevel ? "Limit Energy"
        : props.type == LevelTypeEnum.MultipleClickLevel ? "Multiple Click"
            : props.type == LevelTypeEnum.RechargeEnergyLevel ? "Recharge Energy"
                : "Auto Click";
};
const getCurrenctLevel = () => {
    if (props.user) {
        return props.type == LevelTypeEnum.LimitEnergyLevel ? props.user.limitEnergyLevel.level
            : props.type == LevelTypeEnum.MultipleClickLevel ? props.user.multipleClickLevel.level
                : props.type == LevelTypeEnum.RechargeEnergyLevel ? props.user.rechargeEnergyLevel.level
                    : props.user.roBotLevel ? props.user.roBotLevel.level : "0";
    } else return 0;
};
const confirmUpgrade = () => {
    const getLevel = getUpgradeLevel();

    if (!isSelected || !getLevel || !props.user) return;

    if (getLevel.availableCoin > props.user.balanceCoin) {
        createNotification({
            title: "Balance Error",
            description: "Balance is not enough",
            type: NotificationTypeEnum.Exception,
        });
        return;
    }
    createNotification({
        title: "please wait",
        description: "The request is in progress ...",
        type: NotificationTypeEnum.Waiting,
    });
    
    levelService.levelUp(props.type)
        .then(result => {
            createNotification({
                title: "Successful LevelUp",
                description: "The LevelUp was made successfully",
                type: NotificationTypeEnum.Success,
            });
            emit("upgrade");
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
                title: "Level Up Error",
                description: "Can not level up now",
                type: NotificationTypeEnum.Exception,
            });
        })
    isSelected.value = false;
};


</script>
<style scoped>
.boost_icon {
    width: 50px;
    height: 50px;
}

.max_level {
    opacity: 0.5;
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