with open("Assets/Scripts/Player/PlayerEffectController.cs", "r") as f:
    orig = f.read()

s1 = """        private IEnumerator ReplaceEffectRoutine(PlayerEffect oldEffect, PlayerEffect newEffect)
        {
            var type = newEffect.GetType();
            
            // Aguarda o final do frame para garantir que os cálculos do motor terminem
            // evitando condições de corrida, depois remove o antigo
            yield return new WaitForEndOfFrame();
            oldEffect.Remove(_playerController);
            
            // Depois aplica o novo efeito com segurança
            _activeEffects[type] = new ActiveEffect(newEffect, StartCoroutine(EffectRoutine(newEffect)));
        }"""
s2 = """        private IEnumerator ReplaceEffectRoutine(PlayerEffect oldEffect, PlayerEffect newEffect)
        {
            var type = newEffect.GetType();
            
            // Primeiro removemos o efeito antigo
            oldEffect.Remove(_playerController);
            
            // Aguardamos até o final do frame atual 
            // (isso garante que as rotinas e a física reajustem a remoção do buff)
            yield return new WaitForEndOfFrame();
                        
            // Em seguida aplicamos o novo efeito com segurança,
            // garantindo que não colidam
            _activeEffects[type] = new ActiveEffect(newEffect, StartCoroutine(EffectRoutine(newEffect)));
        }"""

if s1 in orig:
    c = orig.replace(s1, s2)
    with open("Assets/Scripts/Player/PlayerEffectController.cs", "w") as f:
        f.write(c)
    print("Fixed!")
else:
    print("Not found")

