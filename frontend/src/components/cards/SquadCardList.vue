<template>
    <div class="flex-center flex-column">
        <div v-if="squads && squads.length == 0" class="flex-center py-3">
            <span class="text-color fs-small fw-bold">Can not found squads</span>
        </div>

        <SquadCard v-if="squads" v-for="(   squad, index   ) in    squads   " :key="index" :squad="squad" />
        <SquadCard v-else v-for="(   squad, i   ) in    [null, null, null]   " :key="i" :squad="squad" />
    </div>
</template>

<script setup lang="ts">
import type SquadModel from '@/models/squadModel';

import SquadCard from '@/components/cards/SquadCard.vue';
import SkeletonLoader from '@/components/utilities/SkeletonLoader.vue';

interface Props {
    squads: Array<SquadModel> | null
}
const props = defineProps<Props>()

</script>