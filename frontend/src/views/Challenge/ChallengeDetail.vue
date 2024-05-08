<template>
  <div class="mx-auto page_information flex-center flex-column text-center">
    <h1 class="text-color fs-large fw-bold">Challenge</h1>
    <p class="description-color fs-small fw-bold mt-3">
      complet all tasks and receive bonus
    </p>
  </div>
  <section class="section_tasks mt-5">
    <span class="fs-medium fw-bold text-color">challenge tasks</span>
    <div class="d-flex flex-column">
      <div
        v-for="(task, index) in tasks"
        :key="index"
        class="mt-3 bg-card box-shadow p-2 rounded-1"
      >
        <div
          @click="completTaskRequest(task)"
          :class="task.isCompleted ? 'completed' : ''"
          class="d-flex align-items-center justify-content-between"
        >
          <div class="flex-center">
            <div class="card_icon flex-center bg-card mr-2 rounded-1 p-2 box-shadow">
              <img
                v-if="task.isCompleted"
                class="w-full h-full"
                src="@/assets/images/icons/completed.png"
                alt=""
              />
              <img
                v-else
                class="w-full h-full"
                src="@/assets/images/icons/hourglass.png"
                alt=""
              />
            </div>
            <div class="d-flex flex-column">
              <span class="text-color fs-small fw-bold mb-2">{{ task.title }}</span>
              <div class="d-flex align-items-center mr-3">
                <span class="description-color fs-small fw-bold">
                  {{ task.isCompleted ? "completed" : "complet task" }}
                </span>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
  <section class="section_completchallenge mt-5">
    <div class="d-flex flex-column">
      <span class="text-color fs-medium fw-bold">Recive Bonus</span>
      <span class="description-color fs-small fw-bold mt-1"
        >complet tasks and click button
      </span>
    </div>
    <div
      @click="completChallengeRequest"
      class="bg-theme rounded-1 p-2 box-shadow flex-center mt-3"
    >
      <span class="text-color fs-medium fw-bold">claim bonus</span>
    </div>
  </section>
</template>

<script lang="ts" setup>
import { ref, inject } from "vue";
import { useRoute } from "vue-router";
import { NotificationTypeEnum } from "@/models/notificationModel";

import ChallengeService from "@/services/challengesService";
import type TaskModel from "@/models/taskModel";
import router from "@/router";

const route = useRoute();

const tasks = ref<Array<TaskModel>>(new Array<TaskModel>());

const challengeService = new ChallengeService();

const id = route.params.id.toString();

challengeService.getTasks(id).then((result) => (tasks.value = result));
const { createNotification } = inject("notification");

const completTaskRequest = (task: TaskModel) => {
  if (task.isCompleted) return;

  challengeService.completTask(task.id).then((result) => {
    if (result.isCompleted) {
      task.isCompleted = result.isCompleted;
    } else window.location.href = result.path;
  });
};
const checkCompletedAllTasks = () => {
  if (tasks.value) {
    tasks.value.forEach((item) => {
      if (!item.isCompleted) return false;
    });
    return true;
  }
  return false;
};
const completChallengeRequest = () => {
  if (checkCompletedAllTasks()) {
    createNotification({
      title: "please wait ",
      description: `please wait for complet task`,
      type: NotificationTypeEnum.Waiting,
    });
    challengeService.completChallenge(id).then((result) => {
      router.push({ name: "earn" });

      createNotification({
        title: "Challenge Completed",
        description: `Are You received ${result.bonus} coin`,
        type: NotificationTypeEnum.Success,
      });
    });
  } else
    createNotification({
      title: "Tasks Error",
      description: `please complet all tasks`,
      type: NotificationTypeEnum.Exception,
    });
};
</script>

<style scoped>
.completed {
  opacity: 0.5;
}
</style>
