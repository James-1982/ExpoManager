<template>
  <div class="state-chip" :style="{ borderColor: color, color: color }">
    <span class="icon-wrapper">
      <component :is="iconComponent" class="icon"/>
    </span>
    {{ state }}
  </div>
</template>
  
  <script setup>
  import HelpCircleOutline from 'vue-material-design-icons/HelpCircleOutline.vue';
  import Autorenew from 'vue-material-design-icons/Autorenew.vue';
  import PencilOutline from 'vue-material-design-icons/PencilOutline.vue';
  import PauseCircleOutline from 'vue-material-design-icons/PauseCircleOutline.vue';
  import CheckCircleOutline from 'vue-material-design-icons/CheckCircleOutline.vue';
  
  const props = defineProps({
    state: {
      type: String,
      required: true
    }
  });
  
  // Mappa stati → componenti icona
  const stateIcons = {
    Undefined: HelpCircleOutline,
    Generated: Autorenew,
    Draft: PencilOutline,
    Suspended: PauseCircleOutline,
    Approved: CheckCircleOutline
  };
  
  // Mappa stati → colori
  const stateColors = {
    Undefined: 'gray',
    Generated: 'blue',
    Draft: 'orange',
    Suspended: 'red',
    Approved: 'green'
  };
  
  // Calcolo dell’icona e del colore
  const iconComponent = stateIcons[props.state] || HelpCircleOutline;
  const color = stateColors[props.state] || 'gray';
  </script>
  
  <style src="../styles/components/StateChip.css" />