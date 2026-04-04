<template>
  <div class="register-form">

    <div class="form-group">
      <label for="email">Email</label>
      <input id="email" v-model="email" type="email" placeholder="Inserisci la tua email" />
    </div>

    <div class="form-group">
      <label for="password">Password</label>
      <input id="password" v-model="password" type="password" placeholder="Inserisci la tua password" />
    </div>

    <button @click="register" :disabled="loading">
      {{ loading ? 'Registrazione...' : 'Registrati' }}
    </button>

    <p class="message" v-if="message">{{ message }}</p>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import api from '../api';

const email = ref('');
const password = ref('');
const loading = ref(false);
const message = ref('');

const router = useRouter();

const register = async () => {
  loading.value = true;
  message.value = '';

  try {
    await api.post('/Authentication/register', {
      email: email.value,
      password: password.value,
    });

    message.value = 'Registrazione completata!';
    
    // Redirect automatico alla WelcomePage / login
    setTimeout(() => {
      router.push('/');
    }, 1000); // piccolo delay per far leggere il messaggio
  } catch (err: any) {
    console.error(err);
    message.value =
      err.response?.data?.message || 'Errore durante la registrazione';
  } finally {
    loading.value = false;
  }
};
</script>

<style src="../styles/components/RegisterForm.css"></style>