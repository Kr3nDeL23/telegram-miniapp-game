<template>
    <div @click="redirectSquadDetail" class="squad_card py-3 w-full d-flex align-items-center justify-content-between">
        <div class="flex-center">
            <div class="squad_profile d-flex mr-3">
                <img v-if="squad" class="rounded-circle w-full h-full" :src="$filters.serverLinkFormat(squad.image)"
                    alt="">
                <SkeletonLoader v-else width="55px" height="55px" />
            </div>
            <div class="d-flex flex-column">

                <span v-if="squad" class="text-color fs-small fw-bold">{{ $filters.truncate(squad.name, 30)
                    }}</span>
                <SkeletonLoader v-else width="200px" height="20px" />

                <span class="description-color fs-small fw-normal mt-2">
                    <div v-if="squad" class="d-flex align-items-center">
                        <div class="d-flex align-items-center mr-3">
                            <img class="mr-1" width="15px" src="@/assets/images/icons/coin.svg" alt="">
                            {{ $filters.numberFormat(squad.balanceCoin) }}
                        </div>
                        <div class="d-flex align-items-center">
                            <img class="mr-1" width="15px" :src="$filters.serverLinkFormat(squad.league.image)" alt="">
                            {{ squad.league.name }}
                        </div>
                    </div>
                    <SkeletonLoader v-else width="100px" height="20px" />
                </span>

            </div>
        </div>
    </div>
</template>

<script setup lang="ts">
import router from '@/router';
import type SquadModel from '@/models/squadModel';

import SkeletonLoader from '@/components/utilities/SkeletonLoader.vue';

interface Props {
    squad: SquadModel | null
}
const redirectSquadDetail = () => {
    if (props.squad)
        router.push({ name: 'squad_detail', params: { id: props.squad.id } })
};
const props = defineProps<Props>()

</script>

<style scoped>
.squad_profile {
    min-width: 55px;
    min-height: 55px;
    max-width: 55px;
    max-height: 55px;
}

.squad_card {
    border-bottom: 1px solid rgba(var(--border-color), 0.2);
}
</style>