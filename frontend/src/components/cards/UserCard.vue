<template>
    <div class="user_card py-3 w-full d-flex align-items-center justify-content-between">
        <div class="flex-center">
            <div class="user_profile d-flex mr-3">
                <img v-if="user" class="rounded-circle w-full h-full" :src="$filters.serverLinkFormat(user.image)"
                    alt="">
                <SkeletonLoader v-else width="55px" height="55px" />
            </div>
            <div class="d-flex flex-column">

                <span v-if="user" class="text-color fs-small fw-bold">{{ $filters.truncate(user.name, 30)
                    }}</span>
                <SkeletonLoader v-else width="200px" height="20px" />

                <span class="description-color fs-small fw-normal mt-2">
                    <div v-if="user" class="d-flex align-items-center">
                        <div class="d-flex align-items-center mr-3">
                            <img class="mr-1" width="15px" src="@/assets/images/icons/coin.svg" alt="">
                            {{ $filters.numberFormat(user.balanceCoin) }}
                        </div>
                        <div class="d-flex align-items-center">
                            <img class="mr-1" width="15px" :src="$filters.serverLinkFormat(user.league.image)" alt="">
                            {{ user.league.name }}
                        </div>
                    </div>
                    <SkeletonLoader v-else width="100px" height="20px" />
                </span>

            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import type UserModel from '@/models/userModel';

import SkeletonLoader from '@/components/utilities/SkeletonLoader.vue';

interface Props {
    user: UserModel | null
}

const props = defineProps<Props>()

</script>

<style scoped>
.user_profile {
    min-width: 55px;
    min-height: 55px;
    max-width: 55px;
    max-height: 55px;
}

.user_card {
    border-bottom: 1px solid rgba(var(--border-color), 0.2);
}
</style>