<template>
  <div class="page-container">
    <div class="cards-container">

      <CardBase
        v-for="category in categories"
        :key="category.id"
        :item="category">

        <template #extra>
          <div class="extra-row extra-block">
            <!-- Colonna sinistra vuota -->
            <div class="extra-left"></div>

            <!-- Colonna destra con Home Page + switch -->
            <div class="extra-right">
              <span class="extra-label">Home Page</span>
              <label class="switch">
                <input type="checkbox" v-model="category.highlighted" />
                <span class="slider round"></span>
              </label>
            </div>
          </div>

          <!-- Nome -->
          <div class="fs-large">{{ category.name }}</div>
          <!-- Descrizione -->
          <div class="fs-medium">{{ category.description }}</div>
 
        </template>

      </CardBase>
    </div>
  </div>
  </template>
  
  <script lang="ts" setup>
  import { ref, onMounted } from 'vue';
  import api from '../api';
  import CardBase from './CardBase.vue';
    
  interface Category {
    id: number;
    name: string;
    imageUrl?: string | null;
    description?: string | null;
    lastModify: string;
    modifyBy: string;
    highlighted: boolean;
  }
  
  const categories = ref<Category[]>([]);
  
  onMounted(async () => {
    try {
      const res = await api.get('/Category');
      categories.value = res.data;
    } catch (err) {
      console.error(err);
    }
  });
  </script>
  
  <style src="../styles/components/CategoryCard.css"></style>