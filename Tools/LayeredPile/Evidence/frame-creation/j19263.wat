  (func $j19263 (type 1) (param i32 i32 i32)
    (local i32 f32)
    i32.const 4507821
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3485668
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3434420
      call $j870
      i32.const 3435416
      call $j870
      i32.const 3437580
      call $j870
      i32.const 4507821
      i32.const 1
      i32.store8
    end
    local.get 0
    f32.load offset=68
    local.set 4
    block  ;; label = @1
      local.get 4
      f32.const 0x0p+0 (;=0;)
      f32.lt
      if  ;; label = @2
        local.get 0
        local.get 0
        f32.load offset=64
        f32.store offset=68
        local.get 0
        i32.load offset=48
        local.set 2
        local.get 2
        if  ;; label = @3
          i32.const 3437580
          i32.load
          call $j871
          local.get 2
          i32.const 0
          call $j9353
        end
        local.get 0
        i32.load offset=56
        local.set 2
        local.get 2
        if  ;; label = @3
          i32.const 3437580
          i32.load
          call $j871
          local.get 2
          i32.const 0
          call $j9353
        end
        local.get 0
        i32.load offset=120
        local.set 2
        i32.const 3435416
        i32.load
        call $j871
        local.get 2
        i32.const 0
        call $j982
        if  ;; label = @3
          local.get 0
          i32.load offset=120
          local.get 0
          call $j36804
        end
        block  ;; label = @3
          local.get 0
          i32.load8_u offset=94
          i32.eqz
          br_if 0 (;@3;)
          local.get 0
          i32.load offset=96
          local.set 2
          block  ;; label = @4
            local.get 2
            if  ;; label = @5
              i32.const 0
              local.set 3
              local.get 2
              i32.load offset=8
              br_if 1 (;@4;)
            end
            i32.const 1
            local.set 3
          end
          local.get 3
          br_if 0 (;@3;)
          local.get 0
          i32.load offset=96
          local.set 2
          i32.const 3434420
          i32.load
          call $j871
          local.get 2
          i32.const 0
          call $j6671
        end
        local.get 0
        i32.load offset=124
        local.set 2
        i32.const 3435416
        i32.load
        call $j871
        local.get 2
        i32.const 0
        call $j982
        i32.eqz
        br_if 1 (;@1;)
        local.get 0
        i32.load offset=124
        local.set 2
        local.get 2
        local.set 3
        local.get 2
        i32.load
        local.set 2
        local.get 2
        i32.load offset=220
        local.get 3
        local.get 2
        i32.const 32
        i32.add
        i32.load offset=192
        call $redir_vii
        br 1 (;@1;)
      end
      local.get 0
      local.get 4
      f32.store offset=64
    end
    local.get 0
    local.get 1
    i32.store offset=12
    local.get 0
    i32.const 0
    i32.store8 offset=19
    local.get 0
    i32.const 1
    i32.store8 offset=16
    local.get 0
    i32.load offset=20
    local.set 1
    i32.const 3435416
    i32.load
    call $j871
    local.get 1
    i32.const 0
    i32.const 0
    call $j924
    if  ;; label = @1
      local.get 0
      local.get 0
      i32.const 3485668
      i32.load
      call $j1084
      i32.store offset=20
    end
    local.get 0
    i32.load offset=20
    i32.const 1
    i32.const 0
    call $j6539
    local.get 0
    i32.const 0
    i32.store8 offset=92
    local.get 0
    i32.const 0
    i32.store8 offset=17
    local.get 0
    i32.load offset=24
    i32.const 1
    i32.const 0
    call $j907
    local.get 0
    i32.load offset=28
    i32.const 1
    i32.const 0
    call $j907
    local.get 0
    i32.const 0
    i32.store8 offset=18
    i32.const 3434312
    i32.load
    call $j871
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.load
    local.set 1
    i32.const 3435416
    i32.load
    call $j871
    local.get 1
    i32.const 0
    call $j982
    if  ;; label = @1
      i32.const 3434312
      i32.load
      call $j871
      local.get 0
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load
      f32.load offset=64
      f32.store offset=84
      local.get 0
      i32.const 3434312
      i32.load
      i32.load offset=92
      i32.load
      f32.load offset=68
      f32.store offset=88
    end
    local.get 0
    i32.const 1
    local.get 0
    call $j7955
    local.get 0
    i32.const 0
    i32.store8 offset=93
    local.get 0
    i32.load offset=124
    local.set 1
    i32.const 3435416
    i32.load
    call $j871
    local.get 1
    i32.const 0
    call $j982
    if  ;; label = @1
      local.get 0
      i32.load offset=124
      local.set 0
      local.get 0
      local.set 1
      local.get 0
      i32.load
      local.set 0
      local.get 0
      i32.load offset=228
      local.get 1
      local.get 0
      i32.const 40
      i32.add
      i32.load offset=192
      call $redir_vii
    end)