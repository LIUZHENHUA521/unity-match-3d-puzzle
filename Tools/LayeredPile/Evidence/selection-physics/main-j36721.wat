  (func $j36721 (type 0) (param i32 i32)
    i32.const 4507826
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428772
      call $j870 ;; 
      i32.const 3430720
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 3568828
      call $j870 ;; 
      i32.const 4507826
      i32.const 1
      i32.store8
    end
    local.get 0
    i32.load offset=44
    local.set 1
    i32.const 3435416
    i32.load
    call $j871 ;; 
    local.get 1
    i32.const 0
    i32.const 0
    call $j924 ;; Object.op_Equality
    i32.eqz
    if  ;; label = @1
      i32.const 3428772
      i32.load
      call $j871 ;; 
      i32.const 5
      i32.const 0
      i32.const 0
      i32.const 0
      call $j4064 ;; AnimatorMa.GetAnim
      local.set 1
      i32.const 3435416
      i32.load
      call $j871 ;; 
      local.get 1
      i32.const 0
      i32.const 0
      call $j915 ;; Object.op_Inequality
      if  ;; label = @2
        local.get 0
        i32.load offset=44
        local.get 1
        f32.const 0x1.99999ap-3 (;=0.2;)
        i32.const 0
        i32.const 0
        call $j3101 ;; AnimancerComponent.Play
        f32.const 0x0p+0 (;=0;)
        i32.const 0
        call $j2185 ;; AnimancerState.set_Time
        return
      end
      i32.const 3430720
      i32.load
      call $j871 ;; 
      i32.const 3568828
      i32.load
      i32.const 0
      call $j1062 ;; Debug.LogError
    end)
