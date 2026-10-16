  (func $j6079 (type 43) (param i32 f32 f32 i32)
    (local f32)
    i32.const 4526060
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434688
      call $j870
      i32.const 4526060
      i32.const 1
      i32.store8
    end
    local.get 0
    local.get 1
    f32.store offset=36
    local.get 0
    f32.load offset=28
    local.set 4
    local.get 1
    local.get 4
    f32.eq
    if  ;; label = @1
      local.get 1
      f32.const 0x0p+0 (;=0;)
      f32.eq
      if  ;; label = @2
        local.get 0
        local.set 3
        local.get 0
        i32.load
        local.set 0
        local.get 0
        i32.load offset=484
        local.get 3
        local.get 0
        i32.load offset=488
        call $redir_vii
        return
      end
      local.get 0
      i32.const 0
      i32.store offset=40
      local.get 0
      local.set 3
      local.get 0
      i32.load
      local.set 0
      local.get 0
      i32.load offset=476
      local.get 3
      local.get 0
      i32.load offset=480
      call $redir_vii
      return
    end
    local.get 0
    local.set 3
    local.get 2
    f32.const 0x0p+0 (;=0;)
    f32.le
    if  ;; label = @1
      f32.const inf (;=inf;)
      local.set 1
    else
      i32.const 3434688
      i32.load
      call $j871
      local.get 4
      local.get 1
      f32.sub
      f32.abs
      local.get 2
      f32.div
      local.set 1
    end
    local.get 3
    local.get 1
    f32.store offset=40
    local.get 0
    i32.load
    local.set 3
    local.get 3
    i32.load offset=476
    local.get 0
    local.get 3
    i32.load offset=480
    call $redir_vii
    local.get 0
    local.get 0
    call $j6080)