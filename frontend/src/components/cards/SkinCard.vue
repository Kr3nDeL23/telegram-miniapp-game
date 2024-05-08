<template>
    <div :class="user && skin && user.skinId == skin.id ? 'selected' : ''"
        class="mt-3 bg-card box-shadow p-2 rounded-1">
        <div class="d-flex align-items-center justify-content-between">
            <div class="flex-center">
                <div class="skin_icon flex-center bg-card mr-2 rounded-1 p-1 box-shadow">
                    <img v-if="user && skin" class="w-full h-full" :src="$filters.serverLinkFormat(skin.image)" alt="">
                    <SkeletonLoader v-else width="100%" height="100%" />
                </div>
                <div class="d-flex flex-column">
                    <span v-if="user && skin" class="text-color fs-small fw-bold mb-2">{{
        $filters.truncate(skin.title, 30) }}</span>
                    <SkeletonLoader v-else width="70px" height="15px" />

                    <div v-if="user && skin" class="d-flex align-items-center">
                        <img class="mr-1" v-if="!skin.isBought" width="15px" src="@/assets/images/icons/coin.svg" alt="">
                        <span class="text-color fs-small fw-normal">
                            <b class="text-color"></b>
                            {{ skin.isBought ? "bought" : $filters.numberFormat(skin.availableCoin) }}
                        </span>
                    </div>
                    <SkeletonLoader class="mt-2" v-else width="40px" height="15px" />
                </div>
            </div>
            <div class="flex-center">
                <span v-if="user && skin" class="text-color fs-secondary-small fw-normal bg-card py-1 px-2 rounded-2">
                    {{ user.skinId == skin.id ? 'selected' : skin.isBought ? 'select' : 'buy' }}
                </span>
                <SkeletonLoader v-else width="30px" height="15px" />
            </div>
        </div>
    </div>
</template>
<script setup lang="ts">
import { ref } from 'vue';

import SkeletonLoader from '@/components/utilities/SkeletonLoader.vue';


import type SkinModel from '@/models/skinModel';
import type UserModel from '@/models/userModel';

interface Props {
    skin: SkinModel | null,
    user: UserModel | null,
}

const props = defineProps<Props>()


</script>

<style scoped>
.skin_icon {
    width: 50px;
    height: 50px;
}

.selected {
    opacity: 0.5;
}
</style>