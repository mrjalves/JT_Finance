import { initializeApp } from 'https://www.gstatic.com/firebasejs/9.22.0/firebase-app.js';
import {
    getAuth, signInWithEmailAndPassword, signInWithRedirect, getRedirectResult,
    signInWithPopup, GoogleAuthProvider, signOut, onAuthStateChanged
} from 'https://www.gstatic.com/firebasejs/9.22.0/firebase-auth.js';

const firebaseConfig = {
    apiKey: "AIzaSyBMGBmvLhvar7rpys7NDvzEexbH_TZUEJs",
    authDomain: "controlefinanceiro-35e2a.firebaseapp.com",
    projectId: "controlefinanceiro-35e2a",
    storageBucket: "controlefinanceiro-35e2a.firebasestorage.app",
    messagingSenderId: "521428017393",
    appId: "1:521428017393:web:051e7a3bf6edac00980950",
    measurementId: "G-92FBF0J0MP"
};

const app = initializeApp(firebaseConfig);
const auth = getAuth(app);
const googleProvider = new GoogleAuthProvider();

// 🔥 USE O MESMO NOME EM TODOS OS LUGARES
window.FireBase = {
    signInWithEmail: async (email, password) => {
        try {
            const userCredential = await signInWithEmailAndPassword(auth, email, password);
            return {
                success: true,
                user: {
                    uid: userCredential.user.uid,
                    email: userCredential.user.email,
                    displayName: userCredential.user.displayName,
                }
            };
        } catch (error) {
            return { success: false, errorMessage: error.message };
        }
    },

    signInWithGoogle: async () => {
        try {
            await signInWithRedirect(auth, googleProvider);
            // Esta função não terá um 'return' direto, pois a página será recarregada.
            // O resultado será pego pela função abaixo.
            return { success: true };
        } catch (error) {
            return { success: false, errorMessage: error.message };
        }
    },

    // NOVA FUNÇÃO para verificar o resultado do login após o redirecionamento
    getGoogleRedirectResult: async () => {
        try {
            const result = await getRedirectResult(auth);
            if (result && result.user) {
                // Usuário logou com sucesso via redirect
                return {
                    success: true,
                    user: {
                        uid: result.user.uid,
                        email: result.user.email,
                        displayName: result.user.displayName,
                    }
                };
            }
            // Nenhum usuário logado via redirect (carregamento normal da página)
            return { success: false, errorMessage: "No redirect result" };
        } catch (error) {
            return { success: false, errorMessage: error.message };
        }
    },

    signOut: async () => {
        await signOut(auth);
    },

    onAuthStateChanged: (dotNetHelper) => {
        onAuthStateChanged(auth, (user) => {
            if (user) {
                dotNetHelper.invokeMethodAsync('OnAuthStateChanged', {
                    uid: user.uid,
                    email: user.email,
                    displayName: user.displayName
                });
            } else {
                dotNetHelper.invokeMethodAsync('OnAuthStateChanged', null);
            }
        });
    }
};