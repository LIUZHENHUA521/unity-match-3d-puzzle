  (func $j9878 (type 66) (param i32 f32 i32 i32 i32)
    (local f32)
    block  ;; label = @1
      local.get 3
      i32.const 1
      i32.eq
      if  ;; label = @2
        local.get 0
        f32.load offset=304
        local.set 5
        i32.const 1
        local.get 5
        local.get 1
        f32.gt
        local.get 2
        select
        i32.eqz
        br_if 1 (;@1;)
        local.get 2
        if  ;; label = @3
          local.get 0
          local.get 1
          f32.store offset=304
          return
        end
        local.get 5
        local.get 0
        f32.load offset=460
        f32.sub
        local.set 5
        local.get 0
        local.get 5
        local.get 1
        local.get 1
        local.get 5
        f32.lt
        select
        f32.store offset=304
        return
      end
      local.get 0
      f32.load offset=368
      local.set 5
      i32.const 1
      local.get 5
      local.get 1
      f32.gt
      local.get 2
      select
      i32.eqz
      br_if 0 (;@1;)
      local.get 2
      if  ;; label = @2
        local.get 0
        local.get 1
        f32.store offset=368
        return
      end
      local.get 5
      local.get 0
      f32.load offset=460
      f32.sub
      local.set 5
      local.get 0
      local.get 5
      local.get 1
      local.get 1
      local.get 5
      f32.lt
      select
      f32.store offset=368
    end)
