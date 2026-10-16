  (func $j19260 (type 0) (param i32 i32)
    (local i32 i32)
    i32.const 4507822
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3437580
      call $j870 ;; 
      i32.const 4507822
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load8_u offset=52
    local.set 1
    local.get 0
    i32.load offset=48
    local.set 2
    local.get 0
    i32.const 0
    call $j925 ;; Component.get_gameObject
    i32.const 0
    call $j1073 ;; Object.get_name
    local.set 3
    i32.const 3437580
    i32.load
    call $j871 ;; 
    local.get 2
    local.get 1
    local.get 3
    i32.const 0
    call $j3492 ;; SoundManager.PlayEffectSound
    local.get 0
    i32.load8_u offset=61
    if  ;; label = @1
      i32.const 3437580
      i32.load
      call $j871 ;; 
      i32.const 1
      i32.const 0
      call $j17949 ;; SoundManager.PlayerGooseSound
    end)
