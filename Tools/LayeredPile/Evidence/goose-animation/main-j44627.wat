  (func $j44627 (type 0) (param i32 i32)
    (local i32 i32)
    local.get 0
    i32.load offset=32
    i32.load offset=24
    local.set 1
    loop  ;; label = @1
      local.get 1
      i32.const 1
      i32.sub
      local.set 1
      local.get 1
      i32.const -1
      i32.gt_s
      if  ;; label = @2
        local.get 0
        i32.load offset=32
        i32.load offset=12
        local.get 1
        i32.const 2
        i32.shl
        i32.add
        i32.load offset=16
        local.set 2
        local.get 2
        local.set 3
        local.get 2
        i32.load
        local.set 2
        local.get 2
        i32.load offset=484
        local.get 3
        local.get 2
        i32.load offset=488
        call $redir_vii
        br 1 (;@1;)
      end
    end)