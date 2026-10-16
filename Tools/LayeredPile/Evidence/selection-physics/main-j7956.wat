  (func $j7956 (type 116) (param i32 i32 i32 f32 i32) (result f32)
    (local f32)
    i32.const 4507825
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428772
      call $j870 ;; 
      i32.const 3435416
      call $j870 ;; 
      i32.const 4507825
      i32.const 1
      i32.store8
    end
    i32.const 3428772
    i32.load
    call $j871 ;; 
    block  ;; label = @1
      block  ;; label = @2
        local.get 2
        if  ;; label = @3
          i32.const 2
          i32.const 0
          i32.const 0
          i32.const 0
          call $j4064 ;; AnimatorMa.GetAnim
          local.set 2
          i32.const 3435416
          i32.load
          call $j871 ;; 
          local.get 2
          i32.const 0
          i32.const 0
          call $j915 ;; Object.op_Inequality
          i32.eqz
          br_if 2 (;@1;)
          local.get 0
          i32.load offset=44
          local.get 2
          local.get 3
          i32.const 0
          i32.const 0
          call $j3101 ;; AnimancerComponent.Play
          f32.const 0x0p+0 (;=0;)
          local.get 2
          i32.const 0
          call $j2232 ;; AnimationClip.get_length
          i32.const 0
          call $j1345 ;; Random.Range
          i32.const 0
          call $j2185 ;; AnimancerState.set_Time
          br 1 (;@2;)
        end
        i32.const 3
        i32.const 4
        local.get 1
        select
        i32.const 1
        i32.const 0
        i32.const 0
        call $j4064 ;; AnimatorMa.GetAnim
        local.set 2
        i32.const 3435416
        i32.load
        call $j871 ;; 
        local.get 2
        i32.const 0
        i32.const 0
        call $j915 ;; Object.op_Inequality
        i32.eqz
        br_if 1 (;@1;)
        local.get 0
        i32.load offset=44
        local.get 2
        local.get 3
        i32.const 0
        i32.const 0
        call $j3101 ;; AnimancerComponent.Play
        f32.const 0x0p+0 (;=0;)
        i32.const 0
        call $j2185 ;; AnimancerState.set_Time
      end
      local.get 2
      i32.const 0
      call $j2232 ;; AnimationClip.get_length
      local.set 5
    end
    local.get 5
    return)
