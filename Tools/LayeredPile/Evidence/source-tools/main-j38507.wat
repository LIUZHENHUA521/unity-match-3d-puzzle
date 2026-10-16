  (func $j38507 (type 81) (param i32 i32 i32 i32 i32) (result f32)
    (local f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32 f32)
    local.get 2
    f32.load
    local.set 10
    local.get 3
    f32.load
    local.get 10
    f32.sub
    local.set 5
    local.get 2
    f32.load offset=4
    local.set 11
    local.get 3
    f32.load offset=4
    local.get 11
    f32.sub
    local.set 6
    local.get 2
    f32.load offset=8
    local.set 12
    local.get 3
    f32.load offset=8
    local.get 12
    f32.sub
    local.set 7
    local.get 1
    f32.load
    local.set 13
    local.get 13
    local.get 10
    f32.sub
    local.set 9
    local.get 1
    f32.load offset=4
    local.set 14
    local.get 14
    local.get 11
    f32.sub
    local.set 16
    local.get 1
    f32.load offset=8
    local.set 15
    local.get 15
    local.get 12
    f32.sub
    local.set 17
    i32.const 4509458
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434696
      call $j870
      i32.const 4509458
      i32.const 1
      i32.store8
    end
    local.get 5
    local.get 5
    f32.mul
    local.get 6
    local.get 6
    f32.mul
    f32.add
    local.get 7
    local.get 7
    f32.mul
    f32.add
    local.set 8
    block  ;; label = @1
      local.get 8
      i32.const 3434696
      i32.load
      i32.load offset=92
      f32.load
      f32.lt
      if  ;; label = @2
        i32.const 4525920
        i32.load8_u
        i32.eqz
        if  ;; label = @3
          i32.const 3439908
          call $j870
          i32.const 4525920
          i32.const 1
          i32.store8
        end
        i32.const 3439908
        i32.load
        i32.load offset=92
        local.set 0
        local.get 0
        f32.load offset=8
        local.set 5
        local.get 0
        f32.load offset=4
        local.set 6
        local.get 0
        f32.load
        local.set 9
        br 1 (;@1;)
      end
      local.get 5
      local.set 18
      local.get 9
      local.get 5
      f32.mul
      local.get 16
      local.get 6
      f32.mul
      f32.add
      local.get 17
      local.get 7
      f32.mul
      f32.add
      local.set 5
      local.get 18
      local.get 5
      f32.mul
      local.get 8
      f32.div
      local.set 9
      local.get 6
      local.get 5
      f32.mul
      local.get 8
      f32.div
      local.set 6
      local.get 7
      local.get 5
      f32.mul
      local.get 8
      f32.div
      local.set 5
    end
    local.get 10
    local.get 9
    f32.add
    local.set 7
    local.get 11
    local.get 6
    f32.add
    local.set 6
    local.get 12
    local.get 5
    f32.add
    local.set 8
    i32.const 4509457
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3434688
      call $j870
      i32.const 4509457
      i32.const 1
      i32.store8
    end
    local.get 13
    local.get 7
    f32.sub
    local.set 5
    local.get 14
    local.get 6
    f32.sub
    local.set 6
    local.get 15
    local.get 8
    f32.sub
    local.set 7
    i32.const 3434688
    i32.load
    call $j871
    local.get 5
    local.get 5
    f32.mul
    local.get 6
    local.get 6
    f32.mul
    f32.add
    local.get 7
    local.get 7
    f32.mul
    f32.add
    f32.sqrt
    return)